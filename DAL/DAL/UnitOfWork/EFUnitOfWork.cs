using DAL.EF;
using DAL.Entities;
using DAL.Repositories.Impl;
using DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        private UserRepository userRepository;
        private TaskRepository tasksRepository;
       
        public EFUnitOfWork(DbContextOptions options)
        {
            _db = new AppDbContext(options);
     
        }

        public ITaskRepository Tasks
        {
            get
                { 
                {
                  if (tasksRepository == null)
                    tasksRepository = new TaskRepository(_db);
                    return tasksRepository;
                }
            }
        }

        public IUserRepository Users
        {
            get
            {
                {
                    if (userRepository == null)
                        userRepository = new UserRepository(_db);
                    return userRepository;
                }
            }
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
