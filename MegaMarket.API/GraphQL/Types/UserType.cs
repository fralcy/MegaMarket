using MegaMarket.Data.Models;

namespace MegaMarket.API.GraphQL.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.Description("Nhân viên trong hệ thống");

        descriptor
            .Field(u => u.UserId)
            .Description("ID của nhân viên");

        descriptor
            .Field(u => u.FullName)
            .Description("Họ tên đầy đủ");

        descriptor
            .Field(u => u.Username)
            .Description("Tên đăng nhập");

        // Không expose password ra ngoài
        descriptor
            .Ignore(u => u.Password);

        descriptor
            .Field(u => u.Role)
            .Description("Vai trò (Admin/Employee)");

        descriptor
            .Field(u => u.Phone)
            .Description("Số điện thoại");

        descriptor
            .Field(u => u.Email)
            .Description("Email");

        // Navigation properties
        descriptor
            .Field(u => u.Attendances)
            .Description("Danh sách chấm công của nhân viên");
    }
}