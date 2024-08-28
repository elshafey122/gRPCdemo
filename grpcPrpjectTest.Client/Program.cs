using Grpc.Core;
using Grpc.Net.Client;
using System.Security.Cryptography.X509Certificates;

namespace grpcPrpjectTest.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //calling server greeter
            var channel = GrpcChannel.ForAddress("https://localhost:7224");
            var client = new Greeter.GreeterClient(channel);


            var hellorequest = new HelloRequest { Name = "ali" };
            var replay0 = await client.SayHelloAsync(hellorequest);
            Console.WriteLine(replay0.Message);



            ////////////////////////////////////////////////



            //calling server todo
            var todochannel = GrpcChannel.ForAddress("https://localhost:7224");
            var clienttodo = new Todo.TodoClient(todochannel);

            // create new todo
            var createtodo = new CreateToDoRequest { Title = "task1", Description = "do homework" };
            var createtodo2 = new CreateToDoRequest { Title = "task2", Description = "do exercise" };
            var replay = clienttodo.CreateToDo(createtodo);
            var replay2 = clienttodo.CreateToDo(createtodo2);
            Console.WriteLine(replay.Id);
            Console.WriteLine(replay2.Id);

             
            //get todo
            var getTodo = new ReadToDoRequest { Id = 1 };
            var replay3 = await clienttodo.ReadToDoAsync(getTodo);
            Console.WriteLine($"id is {replay3.Id} title is {replay3.Title} description is {replay3.Description} todo status is {replay3.ToDoStatus}");

           //get todo list
           var getListTodo = new GetAllRequest();
            var replay4 = await clienttodo.ListToDoAsync(getListTodo);
            foreach (var item in replay4.ToDo)
            {
                Console.WriteLine($"id is {item.Id} title is {item.Title} description is {item.Description} todo status is {item.ToDoStatus}");
            }

            //update todoitem
            var updatetodo = new UpdateToDoRequest()
            {
                Id = 1,
                Title = "newTest",
                Description = "newTest",
                ToDoStatus = "newTest",
            };
            var replay5 = await clienttodo.UpdateToDoAsync(updatetodo);
            Console.WriteLine(replay5.Id);


            //delete todo
            var deletetodo = new DeleteToDoRequest { Id = 1 };
            var replay6 = await clienttodo.DeleteToDoAsync(deletetodo);
            Console.WriteLine(replay6.Id);


            var stramtest = clienttodo.streamListToDo(new GetAllRequest());
            while (await stramtest.ResponseStream.MoveNext())
            {
                var current = stramtest.ResponseStream.Current;
                Console.WriteLine($"id is {current.Id} title is {current.Title} description is {current.Description} todo status is {current.ToDoStatus}");
            }

        }
    }
}
