using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLy_15_UNETI15_TI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Đã comment lại để EF Core không cố gắng tạo thêm cột đã có sẵn trong Database
            // migrationBuilder.AddColumn<bool>(
            //     name: "TienCocDaXacNhan",
            //     table: "DatPhongs",
            //     type: "bit",
            //     nullable: false,
            //     defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Comment phần này để nếu có rollback (quay lùi) cũng không bị lỗi
            // migrationBuilder.DropColumn(
            //     name: "TienCocDaXacNhan",
            //     table: "DatPhongs");
        }
    }
}