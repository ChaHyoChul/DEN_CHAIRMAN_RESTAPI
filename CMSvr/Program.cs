using CMSvr.Infrastructure.Services;
using CMSvr.Infrastructure.VatechService;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Add services to the container.
string tagName = builder.Configuration["TAG_NAME"] ?? "NONE";
builder.Services.AddSingleton<SharedMemoryService>(sp => new SharedMemoryService(tagName));
builder.Services.AddSingleton<IpcQueueService>(sp => new IpcQueueService("IPC_SERVER", tagName, 10));
builder.Services.AddSingleton<MachineStatusService>();
builder.Services.AddSingleton<V1MachineStatusService>();
builder.Services.AddSingleton<V1NcFileService>();
builder.Services.AddSingleton<MachineControlService>();
builder.Services.AddSingleton<NcFileService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
