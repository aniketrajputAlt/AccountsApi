using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccountsApi.Model
{

    // accounts , beneficiary, transaction , acounttype, branchs , 

    [Table("DocTypes")]
    [PrimaryKey("DocTypeId")]
    public class DocType
    {
        [Key]
   
        public int DocTypeId { get; set; }

        [Required]
        [MaxLength(255)]
        public string DocName { get; set; }

        public bool IsActive { get; set; }
    }

    [Table("Documents")]
    [PrimaryKey("DocId")]
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocId { get; set; }

        [Required]
        public byte[] Documents { get; set; }

        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

       // public virtual Customer Customer { get; set; }

        [ForeignKey("DocType")]
        public int DocTypeId { get; set; }

        //public virtual DocType DocType { get; set; }

        public bool IsActive { get; set; }
    }
    [Table("Roles")]
    [PrimaryKey("RoleId")]
    [Index("RoleName", IsUnique = true, Name = "IDX_Roles_Names")]
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; }
    }

    [Table("Customers")]
    [PrimaryKey("CustomerId")]
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; } = 1;

        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string AddressLine1 { get; set; }

        [MaxLength(50)]
        public string AddressLine2 { get; set; }

        [MaxLength(50)]
        public string AddressLine3 { get; set; }

        [Required]
        public int Pincode { get; set; }

        [Required]
        [MaxLength(10)]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(50)]
        public string City { get; set; }

        [Required]
        [MaxLength(50)]
        public string Country { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

      //  public virtual User User { get; set; }
    }

    [Table("Users")]
    [PrimaryKey("UserId")]
    [Index("Username", IsUnique = true, Name = "IDX_User_Names")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]

        public string Username { get; set; }

        [Required]
        [MaxLength(50)]
        public string Password { get; set; }

        public DateTime? LastPasswordChange { get; set; }

        public bool IsActive { get; set; }

        [ForeignKey("Role")]
        public int RoleId { get; set; }

      //  public virtual Role Role { get; set; }
    }

    [Table("Accounts")]
        [PrimaryKey("AccountId")]
        public class Account
        {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public long AccountId { get; set; }
            
            [Required]
            public decimal Balance { get; set; }

            [Required]
            public int wd_Quota { get; set; }

            [Required]
            public int dp_Quota { get; set; }

            [Required]
            public bool isActive { get; set; }

            [Required]
            public int CustomerID { get; set; }

            [Required]
            public int TypeID { get; set; }

            [Required]
            [StringLength(11)]
            public string BranchID { get; set; }

           [ForeignKey("CustomerID")]
           public Customer Customer {  get; set; }

            [ForeignKey("BranchID")]
            public Branch Branch { get; set; }

            [ForeignKey("TypeID")]
            public AccountType AccountType { get; set; }


        }

    [Table("Branches")]
    [PrimaryKey("BranchID")]
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [StringLength(11)]
        public string BranchID { get; set; }

        [Required]
        [StringLength(50)]
        public string BranchName { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }

    [Table("AccountTypes")]
    [PrimaryKey("TypeID")]
    public class AccountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TypeID { get; set; }

        [Required]
        [StringLength(20)]
        public string TypeName { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
    [Table("Beneficiaries")]
    [PrimaryKey("BenefID")]
    public class Beneficiary
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BenefID { get; set; }

        [Required]
        public string BenefName { get; set; }

        [Required]
        public long BenefAccount { get; set; }

        [Required]
        public string BenefIFSC { get; set; }


        [Required]
        public long AccountId { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }

    }
    public class AccountInputModel
    {
        public decimal Balance { get; set; }
        public int CustomerID { get; set; }
        public int TypeID { get; set; }
        public string BranchID { get; set; }
    }


    public class BeneficiaryInputModel
    {
        [Required]
        public string BenefName { get; set; }

        [Required]
        public long BenefAccount { get; set; }

        [Required]
        public string BenefIFSC { get; set; }

        [Required]
        public long AccountId { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
    [Table("Transactions")]
    [PrimaryKey("TransactionID")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionID { get; set; }

        [Column(TypeName = "money")]
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public long Source_acc { get; set; }

        [Required]
        public long Dest_acc { get; set; }
    }
    public class BankingAppDbContext : DbContext
        {
            public DbSet<Account> Accounts { get; set; }
            public DbSet<Branch> Branches { get; set; }
            public DbSet<AccountType> AccountTypes { get; set; }

             public DbSet<Transaction> Transactions {  get; set; }
           public DbSet<Beneficiary> Beneficiaries {  get; set; }
   
        public DbSet<User> Users {  get; set; }

        public DbSet<DocType> DocTypes { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<Customer> Customers {  get; set; }

        public DbSet<Role> Roles {  get; set; }

        public BankingAppDbContext(DbContextOptions<BankingAppDbContext> options)
         : base(options)
        {
        }


        /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
         {
             optionsBuilder.UseSqlServer
                 (@"Server=(local)\MSSQLSERVERNEW;database=NewBankingApp;integrated security=sspi;trustservercertificate=true;multipleactiveresultsets=true");
         }*/

    }

}
