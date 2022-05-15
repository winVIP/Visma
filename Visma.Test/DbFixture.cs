using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visma.DataAccess;

namespace Visma.Test
{
    public class DbFixture
    {
        public VismaDBContext context { get; private set; }

        public DbFixture()
        {
            DbContextOptions<VismaDBContext> options =
                new DbContextOptionsBuilder<VismaDBContext>()
                .UseInMemoryDatabase(databaseName: "VismaDB")
                .UseLazyLoadingProxies()
                .Options;

            VismaDBContext context = new VismaDBContext(options);

            context.Database.EnsureCreated();

            this.context = context;
        }
    }
}
