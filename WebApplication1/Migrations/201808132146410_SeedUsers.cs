namespace WebApplication1.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SeedUsers : DbMigration
    {
        public override void Up()
        {
            Sql(@"
                INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'604b73ad-57e4-47a8-89a5-03576a973f93', N'tobe.onyema@gmail.com', 0, N'AOnd9fwR5IHepzpdYFpglMCeRqKVGGac7Bx2Ppnvaf1CG+b1wf38OP9kcM7Azq0VfQ==', N'090b25d8-1d66-479c-b8bd-e249947dd456', NULL, 0, 0, NULL, 1, 0, N'tobe.onyema@gmail.com')
                INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'b69a675a-4337-4045-a3ac-4b37e56f8c7e', N'russ@gmail.com', 0, N'AIoJIIGeZ9E4HUac0I1om0/LFTUnYwBfHp1G10eUVA2lTJtyynpPutP/T7o+Vp0wyw==', N'85d8a5ac-53ed-4190-89de-da3a572d1a41', NULL, 0, 0, NULL, 1, 0, N'russ@gmail.com')
                INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'c31de93d-02b3-46ef-a451-b3c769a5b97d', N'ebubegeorge7@gmail.com', 0, N'AM8EFCdnbxN/pBdKs+6jvHbRaFeSPuRJmxxLvvC/b+eRPQIAWVLhrrahJxhxVuh3DA==', N'0cac19e6-46e7-4906-9c42-bb636a02de0a', NULL, 0, 0, NULL, 1, 0, N'ebubegeorge7@gmail.com')
                INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'b69a675a-4337-4045-a3ac-4b37e56f8c7e', N'4cff1188-622e-46af-b613-0aa4332593e9')
   
            ");
        }
        
        public override void Down()
        {
        }
    }
}
