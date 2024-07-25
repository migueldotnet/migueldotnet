namespace BikeStore.Extensions
{
    public static class AuthorizationPolicies
    {
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(configure =>
            {
                configure.AddPolicy("AdminOnly", policy =>
                {
                    policy.RequireRole("Admin");
                });
            });

            return services;
        }
    }
}
