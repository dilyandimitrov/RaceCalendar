using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Options;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services;
using RaceCalendar.Domain.Services.Interfaces;
using RaceCalendar.Infrastructure.Commands;
using RaceCalendar.Infrastructure.Persistence;
using RaceCalendar.Infrastructure.Queries;

namespace RaceCalendar.Api.Configuration
{
    internal static class StartupExtensions
    {
        public static IServiceCollection AddRaceCalendarInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);

            services.AddQueries();
            services.AddCommands();
            services.AddServices(configuration);

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<ConnectionStringsOptions>()
                .Bind(configuration.GetSection("ConnectionStrings"))
                .ValidateDataAnnotations();

            services.AddScoped<IConnectionProvider, ConnectionProvider>();

            return services;
        }

        private static IServiceCollection AddQueries(this IServiceCollection services)
        {
            services.AddScoped<IGetSystemInfoQuery, GetSystemInfoQuery>();
            services.AddScoped<ISearchRacesQuery, SearchRacesQuery>();
            services.AddScoped<ISearchRaceDistancesQuery, SearchRaceDistancesQuery>();
            services.AddScoped<IGetRaceRequestsQuery, GetRaceRequestsQuery>();
            services.AddScoped<IGetRacesByNameIdsQuery, GetRacesByNameIdsQuery>();
            services.AddScoped<IGetRaceDistancesCountQuery, GetRaceDistancesCountQuery>();
            services.AddScoped<IGetRaceDistancesQuery, GetRaceDistancesQuery>();
            services.AddScoped<IGetRaceInfosQuery, GetRaceInfosQuery>();
            services.AddScoped<IGetAllUsersQuery, GetAllUsersQuery>();
            services.AddScoped<IGetUserRacesByUserQuery, GetUserRacesByUserQuery>();
            services.AddScoped<IGetUserSettingsQuery, GetUserSettingsQuery>();
            services.AddScoped<IGetRacesByRaceIdsQuery, GetRacesByRaceIdsQuery>();
            services.AddScoped<IGetImportDataQuery, GetImportDataQuery>();
            services.AddScoped<IGetGeoDataQuery, GetGeoDataQuery>();
            services.AddScoped<IGetAllUpcomingEventsQuery, GetAllUpcomingEventsQuery>();
            services.AddScoped<IGetEventQuery, GetEventQuery>();
            services.AddScoped<IGetEventCommentsQuery, GetEventCommentsQuery>();
            services.AddScoped<IGetEventCommentQuery, GetEventCommentQuery>();

            return services;
        }

        private static IServiceCollection AddCommands(this IServiceCollection services)
        {
            services.AddScoped<ICreateRaceRequestCommand, CreateRaceRequestCommand>();
            services.AddScoped<IMarkRaceRequestAsProcessedCommand, MarkRaceRequestAsProcessedCommand>();
            services.AddScoped<ICreateDefaultUserSettingsCommand, CreateDefaultUserSettingsCommand>();
            services.AddScoped<IUpdateUserSettingsCommand, UpdateUserSettingsCommand>();
            services.AddScoped<IDeleteUserRaceCommand, DeleteUserRaceCommand>();
            services.AddScoped<ICreateUserRaceCommand, CreateUserRaceCommand>();
            services.AddScoped<IUpdateUserRaceCommand, UpdateUserRaceCommand>();
            services.AddScoped<ICreateOrUpdateSystemInfoCommand, CreateOrUpdateSystemInfoCommand>();
            services.AddScoped<ICreateImportDataCommand, CreateImportDataCommand>();
            services.AddScoped<ICreateRaceCommand, CreateRaceCommand>();
            services.AddScoped<IUpdateRaceCommand, UpdateRaceCommand>();
            services.AddScoped<ICreateRaceDistanceCommand, CreateRaceDistanceCommand>();
            services.AddScoped<IUpdateRaceDistanceCommand, UpdateRaceDistanceCommand>();
            services.AddScoped<ICreateRaceInfoCommand, CreateRaceInfoCommand>();
            services.AddScoped<IUpdateRaceInfoCommand, UpdateRaceInfoCommand>();
            services.AddScoped<IDeleteAllRacesCommand, DeleteAllRacesCommand>();
            services.AddScoped<IDeleteUserSettingsCommand, DeleteUserSettingsCommand>();
            services.AddScoped<IDeleteRaceInfosCommand, DeleteRaceInfosCommand>();
            services.AddScoped<IDeleteRaceDistanceCommand, DeleteRaceDistanceCommand>();
            services.AddScoped<IDeleteRaceCommand, DeleteRaceCommand>();
            services.AddScoped<ICreateEventCommand, CreateEventCommand>();
            services.AddScoped<IUpdateEventCommand, UpdateEventCommand>();
            services.AddScoped<IDeleteEventCommand, DeleteEventCommand>();
            services.AddScoped<ICreateEventCommentCommand, CreateEventCommentCommand>();
            services.AddScoped<IDeleteEventCommentCommand, DeleteEventCommentCommand>();
            services.AddScoped<IUpdateEventCommentCommand, UpdateEventCommentCommand>();
            services.AddScoped<IUpdateEventVisitorsCountCommand, UpdateEventVisitorsCountCommand>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMailSender, MailSender>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISearchRacesValidatorService, SearchRacesValidatorService>();
            services.AddScoped<ISearchRacesService, SearchRacesService>();
            services.AddScoped<IRaceRequestService, RaceRequestService>()
                .AddOptions<RaceRequestServiceOptions>()
                .Bind(configuration.GetSection(nameof(RaceRequestServiceOptions)))
                .ValidateDataAnnotations();
            services.AddScoped<IRaceService, RaceService>();
            services.AddScoped<IUserRaceService, UserRaceService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();
            services.AddScoped<ITransliterationService, TransliterationService>();
            services.AddScoped<IUserResultService, UserResultService>();
            services.AddScoped<IImportRaceService, ImportRaceService>();
            services.AddScoped<IExcelUpdaterService, ExcelUpdaterService>();
            services.AddScoped<ISystemInfoService, SystemInfoService>();
            services.AddScoped<IGeoDataService, GeoDataService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ISearchEventsService, SearchEventsService>();
            services.AddScoped<IEventCommentService, EventCommentService>();
            services.AddScoped<ICreateEventCommentMessagingService, CreateEventCommentMessagingService>();

            return services;
        }
    }
}
