using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Plugin.CloudFirestore;
using ToDoApp.Models;
using ToDoApp.Services.AnalyticsService;

namespace ToDoApp.Repositories.FirestoreRepository
{
    public class TaskCategoriesRepository : IFirestoreRepository<TaskCategoryModel>
    {
        protected IAnalyticsService AnalyticsService { get; private set; }
        private readonly string _collectionPath = TaskCategoryModel.CollectionPath;

        public TaskCategoriesRepository(IAnalyticsService analyticsService)
        {
            AnalyticsService = analyticsService;
        }

        public TaskCategoryModel Get()
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
            var query = CrossCloudFirestore.Current
                .Instance
                .Collection(_collectionPath)
                .WhereIn("id", ids)
                .WhereEqualsTo("userId", userId);

            return query;
        }

        public IQuery GetAllContains(string userId, string field, object value)
        {
            var query = CrossCloudFirestore.Current
                .Instance
                .Collection(_collectionPath)
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

        public async Task<bool> Update(TaskCategoryModel model)
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
                    { "Method", "TaskCategoriesRepository.Update()" }
                });

                return false;
            }
        }

        public async Task<bool> Add(TaskCategoryModel model)
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
                    { "Method", "TaskCategoriesRepository.Add()" }
                });

                return false;
            }
        }

        public async Task<bool> Delete(TaskCategoryModel model)
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
                    { "Method", "TaskCategoriesRepository.Delete()" }
                });

                return false;
            }
        }
    }
}