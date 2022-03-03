using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using ToDoApp.Models;
using ToDoApp.Services.AnalyticsService;

namespace ToDoApp.Repositories.FirestoreRepository
{
    public class TasksRepository : IFirestoreRepository<TaskModel>
    {
        protected IAnalyticsService AnalyticsService { get; private set; }
        private readonly string _collectionPath = TaskModel.CollectionPath;

        public TasksRepository(IAnalyticsService analyticsService)
        {
            AnalyticsService = analyticsService;
        }

        public TaskModel Get()
        {
            throw new NotImplementedException();
        }

        public IQuery GetAll(string userId)
        {
            var query = CrossCloudFirestore.Current
                .Instance
                .Collection(_collectionPath)
                .WhereEqualsTo("userId", userId);

            return query;
        }

        public IQuery GetAllByIdArray(string userId, string[] ids)
        {
            throw new NotImplementedException();
        }

        public IQuery GetAllContains(string userId, string field, object value)
        {
            var query = CrossCloudFirestore.Current
                .Instance
                .Collection(_collectionPath)
                //.OrderBy(nameof(TaskModel.CreatedAt), true)
                .WhereEqualsTo(field, value)
                .WhereEqualsTo("userId", userId);

            return query;
        }

        public IQuery GetAllContains(string userId, string field1, object value1, string field2, object value2)
        {
            var query = CrossCloudFirestore.Current
                .Instance
                .Collection(_collectionPath)
                .WhereEqualsTo(field1, value1)
                .WhereEqualsTo(field2, value2)
                .WhereEqualsTo("userId", userId);

            return query;
        }

        public IQuery GetAllContains(string userId, string field1, object value1, string field2, object value2,
            string field3, object value3)
        {
            var query = CrossCloudFirestore.Current
                .Instance
                .Collection(_collectionPath)
                .WhereEqualsTo(field1, value1)
                .WhereEqualsTo(field2, value2)
                .WhereEqualsTo(field3, value3)
                .WhereEqualsTo("userId", userId);

            return query;
        }

        public async Task<bool> Update(TaskModel model)
        {
            try
            {
                await CrossCloudFirestore.Current
                    .Instance
                    .Collection(_collectionPath)
                    .Document(model.Id)
                    .UpdateAsync(model);

                return true;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "TaskRepository.Update()" }
                });

                return false;
            }
        }

        public async Task<bool> Add(TaskModel model)
        {
            try
            {
                await CrossCloudFirestore.Current
                    .Instance
                    .Collection(_collectionPath)
                    .AddAsync(model);

                return true;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "TaskRepository.Add()" }
                });

                return false;
            }
        }

        public async Task<bool> Delete(TaskModel model)
        {
            try
            {
                await CrossCloudFirestore.Current
                    .Instance
                    .Collection(_collectionPath)
                    .Document(model.Id)
                    .DeleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                AnalyticsService.TrackError(ex, new Dictionary<string, string>
                {
                    { "Method", "TaskRepository.Delete()" }
                });

                return false;
            }
        }
    }
}