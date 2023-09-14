using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blog.API.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfPublish = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    User = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfPublish = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RejectionComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    User = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfPublish = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RejectionComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RejectionComments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "AuthorId", "Content", "DateOfPublish", "Status", "Title" },
                values: new object[] { 1, "David Flagg", "d860efca-22d9-47fd-8249-791ba61b07c7", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qu", new DateTime(2023, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Pending approval" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "AuthorId", "Content", "DateOfPublish", "Status", "Title" },
                values: new object[] { 2, "David Flagg", "d860efca-22d9-47fd-8249-791ba61b07c7", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qu", new DateTime(2023, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Published" });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Author", "AuthorId", "Content", "DateOfPublish", "Status", "Title" },
                values: new object[] { 3, "David Flagg", "d860efca-22d9-47fd-8249-791ba61b07c7", "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec qu", new DateTime(2023, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Rejected" });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "DateOfPublish", "PostId", "User", "UserId" },
                values: new object[] { 1, "Take a look at my post @someone", new DateTime(2023, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "David Flagg", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "DateOfPublish", "PostId", "User", "UserId" },
                values: new object[] { 2, "Awesome post!", new DateTime(2023, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Gabriel Rodríguez Castellini", "b7539694-97e7-4dfe-84da-b4256e1ff5c8" });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "DateOfPublish", "PostId", "User", "UserId" },
                values: new object[] { 3, "Rejected by the author", new DateTime(2023, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Emma Flagg", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_RejectionComments_PostId",
                table: "RejectionComments",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "RejectionComments");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
