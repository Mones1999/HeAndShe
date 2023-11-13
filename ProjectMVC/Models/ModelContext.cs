using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProjectMVC.Models;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Discountcoupon> Discountcoupons { get; set; }

    public virtual DbSet<Emailnotification> Emailnotifications { get; set; }

    public virtual DbSet<EmpLessThan500> EmpLessThan500s { get; set; }

    public virtual DbSet<Gender> Genders { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Orderdetail> Orderdetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shoppingcart> Shoppingcarts { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Wallettransaction> Wallettransactions { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = XEPDB1)));User Id=Mones;Password=12345;Persist Security Info=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("MONES")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Categoryid).HasName("SYS_C008905");

            entity.ToTable("CATEGORIES");

            entity.Property(e => e.Categoryid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CATEGORYID");
            entity.Property(e => e.Categoryname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("CATEGORYNAME");
            entity.Property(e => e.Genderid)
                .HasColumnType("NUMBER")
                .HasColumnName("GENDERID");

            entity.HasOne(d => d.Gender).WithMany(p => p.Categories)
                .HasForeignKey(d => d.Genderid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008906");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.Contactid).HasName("SYS_C008961");

            entity.ToTable("CONTACT");

            entity.Property(e => e.Contactid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CONTACTID");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Message)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.Subject)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("SUBJECT");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.HasKey(e => e.Currencyid).HasName("SYS_C008891");

            entity.ToTable("CURRENCIES");

            entity.Property(e => e.Currencyid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CURRENCYID");
            entity.Property(e => e.Currencycode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("CURRENCYCODE");
            entity.Property(e => e.Currencyname)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("CURRENCYNAME");
            entity.Property(e => e.Exchangetobase)
                .HasColumnType("NUMBER")
                .HasColumnName("EXCHANGETOBASE");
        });

        modelBuilder.Entity<Discountcoupon>(entity =>
        {
            entity.HasKey(e => e.Couponid).HasName("SYS_C008952");

            entity.ToTable("DISCOUNTCOUPONS");

            entity.Property(e => e.Couponid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("COUPONID");
            entity.Property(e => e.Couponcode)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("COUPONCODE");
            entity.Property(e => e.Discountpercentage)
                .HasColumnType("NUMBER(5,2)")
                .HasColumnName("DISCOUNTPERCENTAGE");
            entity.Property(e => e.Expirationdate)
                .HasColumnType("DATE")
                .HasColumnName("EXPIRATIONDATE");
        });

        modelBuilder.Entity<Emailnotification>(entity =>
        {
            entity.HasKey(e => e.Notificationid).HasName("SYS_C008926");

            entity.ToTable("EMAILNOTIFICATIONS");

            entity.Property(e => e.Notificationid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("NOTIFICATIONID");
            entity.Property(e => e.Datesent)
                .HasColumnType("DATE")
                .HasColumnName("DATESENT");
            entity.Property(e => e.Message)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("MESSAGE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Emailnotifications)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008927");
        });

        modelBuilder.Entity<EmpLessThan500>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("EMP_LESS_THAN_500");

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("FIRST_NAME");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("LAST_NAME");
            entity.Property(e => e.Salary)
                .HasColumnType("FLOAT")
                .HasColumnName("SALARY");
        });

        modelBuilder.Entity<Gender>(entity =>
        {
            entity.HasKey(e => e.Genderid).HasName("SYS_C008897");

            entity.ToTable("GENDER");

            entity.Property(e => e.Genderid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("GENDERID");
            entity.Property(e => e.Gendername)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("GENDERNAME");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("SYS_C008932");

            entity.ToTable("ORDERS");

            entity.Property(e => e.Orderid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Orderdate)
                .HasColumnType("DATE")
                .HasColumnName("ORDERDATE");
            entity.Property(e => e.Shipmenttrackingnumber)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SHIPMENTTRACKINGNUMBER");
            entity.Property(e => e.Status)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.Totalamount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("TOTALAMOUNT");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008933");
        });

        modelBuilder.Entity<Orderdetail>(entity =>
        {
            entity.HasKey(e => e.Orderdetailid).HasName("SYS_C008935");

            entity.ToTable("ORDERDETAILS");

            entity.Property(e => e.Orderdetailid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ORDERDETAILID");
            entity.Property(e => e.Orderid)
                .HasColumnType("NUMBER")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Priceattimeofpurchase)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICEATTIMEOFPURCHASE");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER")
                .HasColumnName("QUANTITY");

            entity.HasOne(d => d.Order).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008936");

            entity.HasOne(d => d.Product).WithMany(p => p.Orderdetails)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008937");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("SYS_C008945");

            entity.ToTable("PAYMENTS");

            entity.Property(e => e.Paymentid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PAYMENTID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Orderid)
                .HasColumnType("NUMBER")
                .HasColumnName("ORDERID");
            entity.Property(e => e.Paymentdate)
                .HasColumnType("DATE")
                .HasColumnName("PAYMENTDATE");
            entity.Property(e => e.Walletid)
                .HasColumnType("NUMBER")
                .HasColumnName("WALLETID");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Orderid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008946");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Walletid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008947");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Productid).HasName("SYS_C008923");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Productid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Categoryid)
                .HasColumnType("NUMBER")
                .HasColumnName("CATEGORYID");
            entity.Property(e => e.Couponid)
                .HasColumnType("NUMBER")
                .HasColumnName("COUPONID");
            entity.Property(e => e.Description)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Imageurl)
                .HasMaxLength(600)
                .IsUnicode(false)
                .HasColumnName("IMAGEURL");
            entity.Property(e => e.Numberofreviews)
                .HasColumnType("NUMBER")
                .HasColumnName("NUMBEROFREVIEWS");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,1)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Productname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PRODUCTNAME");
            entity.Property(e => e.Productsize)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("PRODUCTSIZE");
            entity.Property(e => e.Rating)
                .HasPrecision(2)
                .HasColumnName("RATING");
            entity.Property(e => e.Status)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.Stockquantity)
                .HasColumnType("NUMBER")
                .HasColumnName("STOCKQUANTITY");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.Categoryid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008924");

            entity.HasOne(d => d.Coupon).WithMany(p => p.Products)
                .HasForeignKey(d => d.Couponid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008953");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("SYS_C008911");

            entity.ToTable("REVIEWS");

            entity.Property(e => e.Reviewid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("REVIEWID");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Rating)
                .HasColumnType("NUMBER")
                .HasColumnName("RATING");
            entity.Property(e => e.Reviewdate)
                .HasColumnType("DATE")
                .HasColumnName("REVIEWDATE");
            entity.Property(e => e.Reviewtext)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("REVIEWTEXT");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PRODUCTID_REVIEWS");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008913");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("SYS_C008889");

            entity.ToTable("ROLES");

            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Rolename)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("ROLENAME");
        });

        modelBuilder.Entity<Shoppingcart>(entity =>
        {
            entity.HasKey(e => e.Cartid).HasName("SYS_C008919");

            entity.ToTable("SHOPPINGCART");

            entity.Property(e => e.Cartid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("CARTID");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Quantity)
                .HasColumnType("NUMBER")
                .HasColumnName("QUANTITY");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Product).WithMany(p => p.Shoppingcarts)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PRODUCTID_SHOPPINGCART");

            entity.HasOne(d => d.User).WithMany(p => p.Shoppingcarts)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008920");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Testimonialid).HasName("SYS_C008955");

            entity.ToTable("TESTIMONIALS");

            entity.Property(e => e.Testimonialid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TESTIMONIALID");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("STATUS");
            entity.Property(e => e.Testimonialcontent)
                .HasMaxLength(2000)
                .IsUnicode(false)
                .HasColumnName("TESTIMONIALCONTENT");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.User).WithMany(p => p.Testimonials)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008956");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("SYS_C008893");

            entity.ToTable("USERS");

            entity.Property(e => e.Userid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Address)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Currencyid)
                .HasColumnType("NUMBER")
                .HasColumnName("CURRENCYID");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .IsUnicode(false)
                .HasColumnName("PASSWORD");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("PHONENUMBER");
            entity.Property(e => e.Registrationdate)
                .HasColumnType("DATE")
                .HasColumnName("REGISTRATIONDATE");
            entity.Property(e => e.Roleid)
                .HasColumnType("NUMBER")
                .HasColumnName("ROLEID");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("USERNAME");

            entity.HasOne(d => d.Currency).WithMany(p => p.Users)
                .HasForeignKey(d => d.Currencyid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008895");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008894");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Walletid).HasName("SYS_C008929");

            entity.ToTable("WALLETS");

            entity.Property(e => e.Walletid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("WALLETID");
            entity.Property(e => e.Balance)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("BALANCE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008930");
        });

        modelBuilder.Entity<Wallettransaction>(entity =>
        {
            entity.HasKey(e => e.Transactionid).HasName("SYS_C008942");

            entity.ToTable("WALLETTRANSACTIONS");

            entity.Property(e => e.Transactionid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("TRANSACTIONID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Transactiondate)
                .HasColumnType("DATE")
                .HasColumnName("TRANSACTIONDATE");
            entity.Property(e => e.Transactiontype)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("TRANSACTIONTYPE");
            entity.Property(e => e.Walletid)
                .HasColumnType("NUMBER")
                .HasColumnName("WALLETID");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Wallettransactions)
                .HasForeignKey(d => d.Walletid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008943");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Wishlistitemid).HasName("SYS_C008915");

            entity.ToTable("WISHLIST");

            entity.Property(e => e.Wishlistitemid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("WISHLISTITEMID");
            entity.Property(e => e.Dateadded)
                .HasColumnType("DATE")
                .HasColumnName("DATEADDED");
            entity.Property(e => e.Productid)
                .HasColumnType("NUMBER")
                .HasColumnName("PRODUCTID");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Product).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PRODUCTID");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("SYS_C008916");
        });
        modelBuilder.HasSequence("CUSTOMER_SEQ");
        modelBuilder.HasSequence("DEPARTMENT_SEQ");
        modelBuilder.HasSequence("EMPLOYEE_SEQ");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
