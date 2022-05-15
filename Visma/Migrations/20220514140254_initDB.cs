using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Visma.Migrations
{
    public partial class initDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmploymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BossID = table.Column<int>(type: "int", nullable: true),
                    HomeAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CurrentSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_Employees_Employees_BossID",
                        column: x => x.BossID,
                        principalTable: "Employees",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExceptionLog",
                columns: table => new
                {
                    ExceptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExceptionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLog", x => x.ExceptionID);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeID", "BirthDate", "BossID", "CurrentSalary", "EmploymentDate", "FirstName", "HomeAddress", "LastName", "Role" },
                values: new object[] { 1, new DateTime(1991, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, 1486.66m, new DateTime(2021, 5, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Culver", "3892 La Follette Drive", "Carde", "CEO" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeID", "BirthDate", "BossID", "CurrentSalary", "EmploymentDate", "FirstName", "HomeAddress", "LastName", "Role" },
                values: new object[,]
                {
                    { 2, new DateTime(1990, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2006.67m, new DateTime(2022, 4, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ashton", "115 Heath Place", "Early", "Legal" },
                    { 3, new DateTime(1995, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1944.54m, new DateTime(2021, 9, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Selma", "750 Tomscot Parkway", "Henrionot", "Training" },
                    { 4, new DateTime(1985, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2608.86m, new DateTime(2021, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "Betteanne", "10636 Marquette Junction", "Leddie", "Services" },
                    { 5, new DateTime(1980, 8, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2810.83m, new DateTime(2021, 11, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Devy", "65874 Park Meadow Plaza", "Quilliam", "Human Resources" },
                    { 9, new DateTime(1994, 3, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2824.65m, new DateTime(2021, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Papagena", "97 Sugar Plaza", "Masding", "Business Development" },
                    { 10, new DateTime(1971, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1264.78m, new DateTime(2021, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dedra", "15 Jenifer Junction", "Kennham", "Marketing" },
                    { 13, new DateTime(1993, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1223.4m, new DateTime(2021, 10, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dewey", "703 Cardinal Street", "Moorrud", "Engineering" },
                    { 14, new DateTime(1994, 3, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1201.54m, new DateTime(2022, 4, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jobi", "209 Blue Bill Park Way", "Francecione", "Research and Development" },
                    { 15, new DateTime(1973, 9, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2528.01m, new DateTime(2021, 6, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "Stace", "0 Hovde Avenue", "Vassar", "Support" },
                    { 19, new DateTime(1977, 12, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1433.01m, new DateTime(2022, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Avrom", "039 Parkside Park", "Noor", "Sales" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeID", "BirthDate", "BossID", "CurrentSalary", "EmploymentDate", "FirstName", "HomeAddress", "LastName", "Role" },
                values: new object[,]
                {
                    { 6, new DateTime(1992, 12, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1520.15m, new DateTime(2021, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dollie", "2 Merry Plaza", "Whissell", "Services" },
                    { 7, new DateTime(1975, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1649.76m, new DateTime(2022, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Maude", "342 Eggendart Place", "Espinay", "Business Development" },
                    { 18, new DateTime(1989, 8, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1038.53m, new DateTime(2021, 11, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gale", "93782 Green Place", "Meddows", "Legal" },
                    { 12, new DateTime(1971, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 1903.95m, new DateTime(2022, 1, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ethel", "39 Eastwood Junction", "Ridgwell", "Training" },
                    { 16, new DateTime(1987, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 2067.06m, new DateTime(2022, 5, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Fanechka", "3 Sullivan Drive", "Cunnington", "Services" },
                    { 8, new DateTime(1975, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1499.01m, new DateTime(2021, 8, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Garey", "59 Parkside Lane", "Maciejak", "Human Resources" },
                    { 17, new DateTime(1991, 12, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 1825.06m, new DateTime(2022, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ogdon", "6 Mandrake Plaza", "Micklewicz", "Human Resources" },
                    { 11, new DateTime(1974, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, 2628.58m, new DateTime(2021, 10, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Quintilla", "5 Blackbird Parkway", "Rupprecht", "Marketing" },
                    { 20, new DateTime(1977, 7, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 13, 1320.31m, new DateTime(2022, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Faulkner", "43 Grover Parkway", "Snuggs", "Engineering" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_BossID",
                table: "Employees",
                column: "BossID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "ExceptionLog");
        }
    }
}
