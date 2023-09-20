using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Contracts.DataModel;

namespace MinimalApi;

internal sealed class MinimalApiContext(DbContextOptions<MinimalApiContext> options)
: IdentityDbContext<UserAccount>(options)
{
}
