using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using ContainerApp.TodoApi.Repository.Interfaces;
using ContainerApp.TodoApi.Repository;
using ContainerApp.TodoApi.Models;

namespace ContainerApp.Test
{
    [TestClass]
    public class TodoItemIntegrationTest
    {
        private ITodoItemRepository _repository;

        [TestInitialize()]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<MyDbContext>()
                .UseInMemoryDatabase(databaseName: "MyDataDatabase")
                .Options;

            _repository = new TodoItemRepository(new MyDbContext(options));
        }

        [TestMethod]
        [TestCategory("Category1")]
        [Priority(1)]
        public void Add_Todo_Items()
        {
            Task.Run(async () =>
            {
                TodoItem todoItem1 = new TodoItem();
                todoItem1.Name = "Todo Item Name 1";
                todoItem1._IsComplete = 0;
                var res1 = await _repository.Add(todoItem1);
                Assert.IsTrue(res1);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Category1")]
        [Priority(1)]
        public void Add_Two_Todo_Items()
        {
            Task.Run(async () =>
            {
                TodoItem todoItem2 = new TodoItem();
                todoItem2.Name = "Todo Item Name 2";
                todoItem2._IsComplete = 0;
                var res2 = await _repository.Add(todoItem2);
                Assert.IsTrue(res2);

                TodoItem todoItem3 = new TodoItem();
                todoItem3.Name = "Todo Item Name 3";
                todoItem3._IsComplete = 1;
                var res3 = await _repository.Add(todoItem3);
                Assert.IsTrue(res3);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Category2")]
        [Priority(2)]
        public void Get_All_Todos()
        {
            Task.Run(async () =>
            {
                var resGetAll = await _repository.GetAll();
                Assert.IsTrue(resGetAll.Count > 0 );
                Assert.AreEqual(resGetAll.ElementAt(0).Name, "Todo Item Name 1");

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Category2")]
        [Priority(2)]
        public void Get_Todo_By_Id()
        {
            Task.Run(async () =>
            {
                var resGet = await _repository.Get(1);
                Assert.IsNotNull(resGet);
                Assert.AreEqual(resGet.Name, "Todo Item Name 1");
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Category2")]
        [Priority(2)]
        public void Delete_Todo_By_Id()
        {
            Task.Run(async () =>
            {
                var res = await _repository.Delete(1);
                Assert.IsTrue(res);
            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Category2")]
        [Priority(2)]
        public void Update_Todo_Item()
        {
            Task.Run(async () =>
            {
                var resGet = await _repository.Get(2);
                resGet.Name = "Todo Item Name Update";
                resGet._IsComplete = 1;
                var res1 = await _repository.Update(resGet);
                Assert.IsTrue(res1);

            }).GetAwaiter().GetResult();
        }

        [TestMethod]
        [TestCategory("Category2")]
        [Priority(2)]
        public void MyTest_Todo_Item()
        {
            Assert.IsTrue(true);
        }
    }
}