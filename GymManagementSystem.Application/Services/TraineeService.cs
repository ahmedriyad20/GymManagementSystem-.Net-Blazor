using GymManagementSystem.DTOs.Trainee.Commands;
using GymManagementSystem.DTOs.Trainee.Queries;
using GymManagementSystem.DTOs.Trainee.Results;
using GymManagementSystem.Entities;
using GymManagementSystem.Entities.Users;
using GymManagementSystem.Enums;
using GymManagementSystem.Interfaces;
using HireAI.Infrastructure.GenaricBasies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.Services
{
    public class TraineeService(
        IRepository<Trainee> traineeRepository,
        IStorageService storageService,
        UserManager<User> userManager) : ITraineeService
    {
        public async Task<List<GetAllTraineesResult>> GetAllAsync(GetAllTraineesQuery query, string currentUserId)
        {
            var isFemaleOnly = await IsFemaleOnlyUserAsync(currentUserId);

            var traineesQuery = traineeRepository.GetAll()
                .Include(t => t.Subscriptions)
                .AsQueryable();

            if (isFemaleOnly)
            {
                traineesQuery = traineesQuery.Where(t => t.Gender == enGender.Female);
            }

            if (query.Gender.HasValue)
            {
                traineesQuery = traineesQuery.Where(t => t.Gender == query.Gender.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchText))
            {
                traineesQuery = traineesQuery.Where(t =>
                    t.Name.Contains(query.SearchText) ||
                    t.Phone.Contains(query.SearchText));
            }

            var trainees = await traineesQuery
                .OrderBy(t => t.Name)
                .ToListAsync();

            return trainees.Select(t =>
            {
                var currentSubscription = t.Subscriptions
                    .OrderByDescending(s => s.EndDate)
                    .FirstOrDefault();

                return new GetAllTraineesResult
                {
                    TraineeId = t.Id,
                    Name = t.Name,
                    Phone = t.Phone,
                    Gender = t.Gender.ToString(),
                    PhotoPath = string.IsNullOrWhiteSpace(t.PhotoPath) ? null : storageService.BuildFileUrl(t.PhotoPath),
                    CurrentSubscriptionEndDate = currentSubscription?.EndDate,
                    RemainingAmount = currentSubscription?.RemainingAmount,
                    CreationTime = t.CreationTime
                };
            }).ToList();
        }

        public async Task<GetTraineeResult?> GetByIdAsync(Guid traineeId, string currentUserId)
        {
            var trainee = await traineeRepository.GetAll()
                .Include(t => t.Subscriptions)
                .Include(t => t.AttendanceSessions)
                .FirstOrDefaultAsync(t => t.Id == traineeId);

            if (trainee is null)
            {
                return null;
            }

            var isFemaleOnly = await IsFemaleOnlyUserAsync(currentUserId);
            if (isFemaleOnly && trainee.Gender != enGender.Female)
            {
                return null;
            }

            return new GetTraineeResult
            {
                TraineeId = trainee.Id,
                Name = trainee.Name,
                Phone = trainee.Phone,
                Gender = trainee.Gender.ToString(),
                PhotoPath = string.IsNullOrWhiteSpace(trainee.PhotoPath) ? null : storageService.BuildFileUrl(trainee.PhotoPath),
                DateOfBirth = trainee.DateOfBirth,
                IsActive = trainee.IsActive,
                CreationTime = trainee.CreationTime,
                Subscriptions = trainee.Subscriptions
                    .OrderByDescending(s => s.StartDate)
                    .Select(s => new TraineeSubscriptionView
                    {
                        SubscriptionId = s.Id,
                        SubscriptionPlan = s.SubscriptionPlan.ToString(),
                        SubscriptionPeriod = s.SubscriptionPeriod.ToString(),
                        TotalAmount = s.TotalAmount,
                        PaidAmount = s.PaidAmount,
                        RemainingAmount = s.RemainingAmount,
                        StartDate = s.StartDate,
                        EndDate = s.EndDate
                    }).ToList(),
                AttendanceSessions = trainee.AttendanceSessions
                    .OrderByDescending(a => a.SessionDateTime)
                    .Select(a => a.SessionDateTime)
                    .ToList()
            };
        }

        public async Task<Guid> CreateAsync(CreateTraineeCommand command)
        {
            string? relativePhotoPath = null;
            if (command.Photo is not null)
            {
                relativePhotoPath = await storageService.UploadAsync(command.Photo, "trainees", withBaseUrl: false);
            }

            var trainee = new Trainee
            {
                Name = command.Name,
                Phone = command.Phone,
                Gender = command.Gender,
                DateOfBirth = command.DateOfBirth,
                IsActive = true,
                PhotoPath = relativePhotoPath
            };

            await traineeRepository.InsertAsync(trainee);
            return trainee.Id;
        }

        public async Task<bool> UpdateAsync(Guid traineeId, UpdateTraineeCommand command)
        {
            var trainee = await traineeRepository.GetAll().FirstOrDefaultAsync(t => t.Id == traineeId);
            if (trainee is null)
            {
                return false;
            }

            trainee.Name = command.Name;
            trainee.Phone = command.Phone;
            trainee.Gender = command.Gender;
            trainee.DateOfBirth = command.DateOfBirth;
            trainee.IsActive = command.IsActive;
            if (command.Photo is not null)
            {
                if (!string.IsNullOrWhiteSpace(trainee.PhotoPath))
                {
                    storageService.Delete(trainee.PhotoPath);
                }

                trainee.PhotoPath = await storageService.UploadAsync(command.Photo, "trainees", withBaseUrl: false);
            }

            await traineeRepository.UpdateAsync(trainee);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid traineeId)
        {
            var trainee = await traineeRepository.GetAll().FirstOrDefaultAsync(t => t.Id == traineeId);
            if (trainee is null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(trainee.PhotoPath))
            {
                storageService.Delete(trainee.PhotoPath);
            }

            await traineeRepository.DeleteAsync(trainee);
            return true;
        }

        private async Task<bool> IsFemaleOnlyUserAsync(string currentUserId)
        {
            var user = await userManager.FindByIdAsync(currentUserId);
            return user is not null && user.Gender == enGender.Female;
        }
    }
}
