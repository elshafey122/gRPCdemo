using Grpc.Core;
using grpcPrpjectTest.Context;
using grpcPrpjectTest.Models;
using grpcPrpjectTest.Protos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static Google.Rpc.Context.AttributeContext.Types;

namespace grpcPrpjectTest.Services
{
    public class TodoService : Todo.TodoBase     // inherit from proto file which has service name
    {
        private readonly AppDbContext _context;
        public TodoService(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(AuthenticationSchemes ="BasicAuth",Roles ="Device")]
        public override async Task<CreateToDoResponse> CreateToDo(CreateToDoRequest request, ServerCallContext context)
        {
            if (request.Title == string.Empty || request.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppply a valid object"));
            }

            var todoitem = new TodoItem
            {
                Description = request.Description,
                Title = request.Title,
            };

            var res = await _context.todoItems.AddAsync(todoitem);
            await _context.SaveChangesAsync();

            return new CreateToDoResponse()
            {
                Id = todoitem.Id
            };

        }

        [Authorize(AuthenticationSchemes = "BasicAuth", Roles = "Device")]
        public override async Task<ReadToDoResponse> ReadToDo(ReadToDoRequest request , ServerCallContext context )
        {
            if (request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));
            }

            var todoitem = await _context.todoItems.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (todoitem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
            }

            return await Task.FromResult( new ReadToDoResponse()
            {
                Id = todoitem.Id,
                Description = todoitem.Description,
                Title = todoitem.Title,
                ToDoStatus = todoitem.ToDoStatus,
            });
        }

        [Authorize(AuthenticationSchemes = "BasicAuth", Roles = "Device")]
        public override async Task<ReadToDoResponse> StreamReadToDo(IAsyncStreamReader<ReadToDoRequest> requestStream, ServerCallContext context)
        {
            //return base.StreamReadToDo(requestStream, context);
            var tssk = Task.Run(async () =>
            {
                await foreach (var item in requestStream.ReadAllAsync())
                {
                    var todoitem = await _context.todoItems.FirstOrDefaultAsync(x => x.Id == item.Id);
                    if (todoitem == null)
                    {
                        throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {item.Id}"));
                    }

                    return await Task.FromResult(new ReadToDoResponse()
                    {
                        Id = todoitem.Id,
                        Description = todoitem.Description,
                        Title = todoitem.Title,
                        ToDoStatus = todoitem.ToDoStatus,
                    });
                }
                return null;
            });
            return null;
        }

        [Authorize(AuthenticationSchemes = "BasicAuth", Roles = "Device")]
        public override async Task<GetAllResponse> ListToDo(GetAllRequest request, ServerCallContext context)
        {
            var response = new GetAllResponse();
            var todoitems = await _context.todoItems.ToListAsync();

            foreach (var todoitem in todoitems)
            {
                response.ToDo.Add(new ReadToDoResponse()
                {
                    Id = todoitem.Id,
                    Description = todoitem.Description,
                    Title = todoitem.Title,
                    ToDoStatus = todoitem.ToDoStatus,
                });
            }
            return await Task.FromResult(response);
        }

        [Authorize(AuthenticationSchemes = "BasicAuth", Roles = "Device")]
        public override async Task streamListToDo(GetAllRequest request, IServerStreamWriter<ReadToDoResponse> responseStream, ServerCallContext context)
        {
            var todoitems = new List<ReadToDoResponse>()
            {
                new ReadToDoResponse
                {
                    Id=6,
                    Description="stream",
                    Title="stream",
                    ToDoStatus = "stream"
                },
                new ReadToDoResponse
                {
                    Id=7,
                    Description="stream",
                    Title="stream",
                    ToDoStatus = "stream"
                }
            };
            foreach (var item in todoitems)
            {
                await Task.Delay(3000);
                var ReadToDoResponse = new ReadToDoResponse
                {
                    Id = item.Id,
                    Description = item.Description,
                    Title = item.Title,
                    ToDoStatus = item.ToDoStatus
                };
                await responseStream.WriteAsync(ReadToDoResponse);
            }
        }

        [Authorize(AuthenticationSchemes = "BasicAuth", Roles = "Device")]
        public override async Task<UpdateToDoResponse> UpdateToDo(UpdateToDoRequest request, ServerCallContext context)
        {

            if (request.Title == string.Empty || request.Description == string.Empty)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "You must suppply a valid object"));
            }

            var todoitem = await _context.todoItems.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (todoitem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
            }

            todoitem.Description = request.Description;
            todoitem.Title = request.Title;
            todoitem.ToDoStatus = request.ToDoStatus;

            await _context.SaveChangesAsync();

            return await Task.FromResult( new UpdateToDoResponse()
            {
                Id = todoitem.Id
            });
        }

        [Authorize(AuthenticationSchemes = "BasicAuth", Roles = "Device")]
        public override async Task<DeleteToDoResponse> DeleteToDo(DeleteToDoRequest request, ServerCallContext context)
        {
            if (request.Id <= 0)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "resouce index must be greater than 0"));
            }
            var todoitem = await _context.todoItems.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (todoitem == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"No Task with id {request.Id}"));
            }
            _context.todoItems.Remove(todoitem);
            await _context.SaveChangesAsync();

            return await Task.FromResult(new DeleteToDoResponse()
            {
                Id = todoitem.Id
            });
        }
    }
}
