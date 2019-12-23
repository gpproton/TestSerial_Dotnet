//
// AppsContext.cs
//
// Author:
//       Godwin peter .O <me@godwin.dev>
//
// Copyright (c) 2019 MIT 2019
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace TestSerial_Dotnet.model
{
    public class AppsContext : DbContext
    {
        private readonly string connectionString;

        public AppsContext()
        {
        }

        public AppsContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public DbSet<Card> Cards { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlite("Data Source=TestDatabase.db;", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<Card>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Id).IsUnique();
                entity.Property(e => e.CaptureTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    public interface ICardProvider
    {
        Card Get(int Id);
    }

    public class CardProvider : ICardProvider
    {
        readonly AppsContext AppsContext;

        CardProvider(AppsContext AppsContext)
        {
            this.AppsContext = AppsContext;
        }

        public Card Get(int Id)
        {
            return AppsContext.Cards.Where(e => e.Id == Id).FirstOrDefault();
        }
    }

}
