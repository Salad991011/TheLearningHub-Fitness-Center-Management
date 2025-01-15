using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TheLearningHub_Fitness_Center_Management.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassPageContent> ClassPageContents { get; set; }


    public virtual DbSet<Contactu> Contactus { get; set; }

    public virtual DbSet<Creditcard> Creditcards { get; set; }

    public virtual DbSet<HomePageContent> HomePageContents { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Paidplan> Paidplans { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Routine> Routines { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SchedulePageContent> SchedulePageContents { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=LAPTOP-40I5EP86)(PORT=1521))(CONNECT_DATA=(SID=orcl)));User Id=C##SALEH; Password=Test321;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("C##SALEH")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Classid).HasName("SYS_C008427");

            entity.ToTable("CLASSES");

            entity.Property(e => e.Classid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CLASSID");
            entity.Property(e => e.Classdate)
                .HasColumnType("DATE")
                .HasColumnName("CLASSDATE");
            entity.Property(e => e.Classdesc)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CLASSDESC");
            entity.Property(e => e.Classname)
                .HasMaxLength(55)
                .IsUnicode(false)
                .HasColumnName("CLASSNAME");
            entity.Property(e => e.Classtime)
                .HasPrecision(6)
                .HasColumnName("CLASSTIME");
            entity.Property(e => e.Imagepath)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("IMAGEPATH");
            entity.Property(e => e.ISAPPROVED)
                    .HasColumnType("NUMBER(1)")
                    .HasDefaultValue(0) // Default value: Not approved
                    .HasColumnName("ISAPPROVED");

            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USERID");
            entity.Property(e => e.APPROVALSTATUS)
               .HasMaxLength(50)
               .IsUnicode(false)
               .HasColumnName("APPROVALSTATUS");

            entity.HasOne(d => d.User).WithMany(p => p.Classes)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("U123");
        });

        modelBuilder.Entity<ClassPageContent>(entity =>
        {
            entity.HasKey(e => e.ClassPageId).HasName("SYS_C008416");

            entity.ToTable("CLASS_PAGE_CONTENT");

            entity.Property(e => e.ClassPageId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CLASS_PAGE_ID");
            entity.Property(e => e.BackgroundDesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_DESC1");
            entity.Property(e => e.BackgroundImagePath1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_IMAGE_PATH1");
            entity.Property(e => e.BackgroundTitle1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_TITLE1");
            entity.Property(e => e.ClassesDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CLASSES_DESC");
            entity.Property(e => e.ClassesTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("CLASSES_TITLE");
        });

        

        modelBuilder.Entity<Contactu>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("SYS_C008412");

            entity.ToTable("CONTACTUS");

            entity.Property(e => e.ContactId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CONTACT_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FNAME");
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LNAME");
            entity.Property(e => e.Msg)
                .IsUnicode(false)
                .HasColumnName("MSG");
            entity.Property(e => e.Subj)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("SUBJ");
        });

        modelBuilder.Entity<Creditcard>(entity =>
        {
            entity.HasKey(e => e.CreditcardId).HasName("SYS_C008403");

            entity.ToTable("CREDITCARD");

            entity.Property(e => e.CreditcardId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("CREDITCARD_ID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER")
                .HasColumnName("BALANCE");
            entity.Property(e => e.CreditcardNum)
                .HasColumnType("NUMBER")
                .HasColumnName("CREDITCARD_NUM");
            entity.Property(e => e.Cvv)
                .HasColumnType("NUMBER")
                .HasColumnName("CVV");
            entity.Property(e => e.ExpirationDate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRATION_DATE");
        });

        modelBuilder.Entity<HomePageContent>(entity =>
        {
            entity.HasKey(e => e.HomePageId).HasName("SYS_C008414");

            entity.ToTable("HOME_PAGE_CONTENT");

            entity.Property(e => e.HomePageId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("HOME_PAGE_ID");
            entity.Property(e => e.BackgroundDesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_DESC1");
            entity.Property(e => e.BackgroundDesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_DESC2");
            entity.Property(e => e.BackgroundImagePath1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_IMAGE_PATH1");
            entity.Property(e => e.BackgroundImagePath2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_IMAGE_PATH2");
            entity.Property(e => e.BackgroundTitle1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_TITLE1");
            entity.Property(e => e.BackgroundTitle2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_TITLE2");
            entity.Property(e => e.CourselDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COURSEL_DESC");
            entity.Property(e => e.CourselItemsDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COURSEL_ITEMS_DESC");
            entity.Property(e => e.CourselItemsTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COURSEL_ITEMS_TITLE");
            entity.Property(e => e.CourselTextTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COURSEL_TEXT_TITLE");
            entity.Property(e => e.FooterEmail)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FOOTER_EMAIL");
            entity.Property(e => e.FooterTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FOOTER_TITLE");
            entity.Property(e => e.FooterTitleDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FOOTER_TITLE_DESC");
            entity.Property(e => e.MainTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("MAIN_TITLE");
            entity.Property(e => e.ServicesDesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SERVICES_DESC1");
            entity.Property(e => e.ServicesDesc2)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SERVICES_DESC2");
            entity.Property(e => e.ServicesImagePath2)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SERVICES_IMAGE_PATH2");
            entity.Property(e => e.ServicesTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SERVICES_TITLE");
            entity.Property(e => e.TrainerDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TRAINER_DESC");
            entity.Property(e => e.TrainerTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TRAINER_TITLE");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.LoginId).HasName("SYS_C008383");

            entity.ToTable("LOGIN");

            entity.Property(e => e.LoginId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("LOGIN_ID");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.RoleId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROLE_ID");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("USER_NAME");

            entity.HasOne(d => d.Role).WithMany(p => p.Logins)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ROLE_ID");
        });

        modelBuilder.Entity<Paidplan>(entity =>
        {

            entity.HasKey(e => e.PlanId).HasName("SYS_C008393");

            entity.ToTable("PAIDPLANS");

            entity.Property(e => e.PlanId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PLAN_ID");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.PlanDesc)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PLAN_DESC");
            entity.Property(e => e.PlanPrice)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PLAN_PRICE");
            entity.Property(e => e.PlanTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PLAN_TITLE");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");
            entity.Property(e => e.TrainerId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TRAINERID");

            entity.HasOne(d => d.User).WithMany(p => p.Paidplans)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PLAN_USER_ID");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("SYS_C008379");

            entity.ToTable("ROLES");

            entity.Property(e => e.RoleId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROLE_ID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ROLE_NAME");
        });

        modelBuilder.Entity<Routine>(entity =>
        {
            entity.HasKey(e => e.RoutineId).HasName("SYS_C008407");

            entity.ToTable("ROUTINES");

            entity.Property(e => e.RoutineId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROUTINE_ID");

            entity.Property(e => e.Desc)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESC_");

            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");

            entity.Property(e => e.RoutineTime)
                .HasColumnType("DATE")
                .HasColumnName("ROUTINE_TIME");

            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USERID"); // Added UserId

            entity.Property(e => e.TrainerId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TRAINERID"); // Added TrainerId

            entity.HasOne(d => d.User)
                .WithMany(p => p.Routines)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ROUTINES_USER_ID"); // Added relationship with User

            entity.HasOne(d => d.Trainer).WithMany()
                .HasForeignKey(d => d.TrainerId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ROUTINE_TRAINER_ID"); // Added relationship with Trainer
        });


        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("SYS_C008423");

            entity.ToTable("SCHEDULE");

            entity.Property(e => e.ScheduleId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SCHEDULE_ID");
            entity.Property(e => e.Day)
                .HasColumnType("DATE")
                .HasColumnName("DAY_");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.PlanId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PLAN_ID");
            entity.Property(e => e.RoutineId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROUTINE_ID");
            entity.Property(e => e.Time)
                .HasPrecision(6)
                .HasColumnName("TIME_");

            entity.HasOne(d => d.Plan).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.PlanId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SCHEDULE_PLAN_ID");

            entity.HasOne(d => d.Routine).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.RoutineId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ROUTINE_PLAN_ID");
        });

        modelBuilder.Entity<SchedulePageContent>(entity =>
        {
            entity.HasKey(e => e.SchedulePageId).HasName("SYS_C008418");

            entity.ToTable("SCHEDULE_PAGE_CONTENT");

            entity.Property(e => e.SchedulePageId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SCHEDULE_PAGE_ID");
            entity.Property(e => e.BackgroundDesc1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_DESC1");
            entity.Property(e => e.BackgroundImagePath1)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_IMAGE_PATH1");
            entity.Property(e => e.BackgroundTitle1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("BACKGROUND_TITLE1");
            entity.Property(e => e.ScheduleDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SCHEDULE_DESC");
            entity.Property(e => e.ScheduleTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SCHEDULE_TITLE");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("SYS_C008405");

            entity.ToTable("SERVICES");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SERVICE_ID");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.ServiceDesc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SERVICE_DESC");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("SERVICE_NAME");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("SYS_C008399");

            entity.ToTable("SUBSCRIPTIONS");

            entity.Property(e => e.SubscriptionId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("SUBSCRIPTION_ID");
            entity.Property(e => e.DateFrom)
                .HasColumnType("DATE")
                .HasColumnName("DATE_FROM");
            entity.Property(e => e.DateTo)
                .HasColumnType("DATE")
                .HasColumnName("DATE_TO");
            entity.Property(e => e.PlanId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("PLAN_ID");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK_SUBSCRIPTIONS_PLAN_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_SUBSCRIPTIONS_USER_ID");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("SYS_C008409");

            entity.ToTable("TESTIMONIALS");

            entity.Property(e => e.TestId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("TEST_ID");
            entity.Property(e => e.IsApproved)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("IS_APPROVED");
            entity.Property(e => e.Rating)
                .HasColumnType("NUMBER")
                .HasColumnName("RATING");
            entity.Property(e => e.TestText)
                .IsUnicode(false)
                .HasColumnName("TEST_TEXT");
            entity.Property(e => e.UserId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TEST_ID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("SYS_C008389");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "SYS_C008390").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER(38)")
                .HasColumnName("USER_ID");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FNAME");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("IMAGE_PATH");
            entity.Property(e => e.Lname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LNAME");
            entity.Property(e => e.LoginId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("LOGIN_ID");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PHONE_NUMBER");
            entity.Property(e => e.RoleId)
                .HasColumnType("NUMBER(38)")
                .HasColumnName("ROLE_ID");

            entity.HasOne(d => d.Login).WithMany(p => p.Users)
                .HasForeignKey(d => d.LoginId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_LOGIN_ID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ROLE_ID_USERS");
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
