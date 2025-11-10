using MegaMarket.Data.Models;

namespace MegaMarket.API.GraphQL.Types;

public class AttendanceType : ObjectType<Attendance>
{
    protected override void Configure(IObjectTypeDescriptor<Attendance> descriptor)
    {
        descriptor.Description("Bản ghi chấm công");

        descriptor
            .Field(a => a.AttendanceId)
            .Description("ID của bản ghi chấm công");

        descriptor
            .Field(a => a.UserId)
            .Description("ID nhân viên");

        descriptor
            .Field(a => a.ShiftTypeId)
            .Description("ID loại ca");

        descriptor
            .Field(a => a.Date)
            .Description("Ngày chấm công");

        descriptor
            .Field(a => a.CheckIn)
            .Description("Thời gian check-in");

        descriptor
            .Field(a => a.CheckOut)
            .Description("Thời gian check-out");

        descriptor
            .Field(a => a.IsLate)
            .Description("Có đi trễ không?");

        descriptor
            .Field(a => a.Note)
            .Description("Ghi chú");

        // Navigation properties
        descriptor
            .Field(a => a.User)
            .Description("Thông tin nhân viên");

        descriptor
            .Field(a => a.ShiftType)
            .Description("Thông tin ca làm việc");
    }
}