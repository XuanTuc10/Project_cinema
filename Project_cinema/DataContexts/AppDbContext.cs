﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Project_cinema.Entities;

namespace Project_cinema.DataContexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<ConfirmEmail> confirmEmails { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<RankCustomer> rankCustomers { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<UserStatus> userStatuses { get; set; }
        public DbSet<Banner> banners { get; set; }
        public DbSet<Bill> bills { get; set; }
        public DbSet<BillFood> billFoods { get; set; }
        public DbSet<BillStatus> billStatuses { get; set; }
        public DbSet<BillTicket> billTickets { get; set; }
        public DbSet<Cinema> cinemas { get; set; }
        public DbSet<Food> foods { get; set; }
        public DbSet<GeneralSetting> generalSettings { get; set; }
        public DbSet<Movie> movies { get; set; }
        public DbSet<MovieType> movieTypes { get; set; }
        public DbSet<Promotion> promotions { get; set; }
        public DbSet<Rate> rates { get; set; }
        public DbSet<Room> rooms { get; set; }
        public DbSet<Schedule> schedules { get; set; }
        public DbSet<Seat> seats { get; set; }
        public DbSet<SeatStatus> seatStatuses { get; set; }
        public DbSet<SeatType> seatTypes { get; set; }
        public DbSet<Ticket> tickets { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = COVINA; Database = Cinema; Trusted_Connection = True; TrustServerCertificate = True;");
        }
    }
}
