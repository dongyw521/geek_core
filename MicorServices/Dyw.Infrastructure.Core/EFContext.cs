using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dyw.Infrastructure.Core
{
    public class EFContext : DbContext
    {
        public EFContext(DbContextOptions options) : base(options)
        {

        }


    }
}
