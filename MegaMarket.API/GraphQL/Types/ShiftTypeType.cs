using MegaMarket.Data.Models;

namespace MegaMarket.API.GraphQL.Types;

public class ShiftTypeType : ObjectType<ShiftType>
{
    protected override void Configure(IObjectTypeDescriptor<ShiftType> descriptor)
    {
        descriptor.Description("Loại ca làm việc");

        descriptor
            .Field(st => st.ShiftTypeId)
            .Description("ID của loại ca");

        descriptor
            .Field(st => st.Name)
            .Description("Tên ca (VD: Ca sáng, Ca chiều)");

        descriptor
            .Field(st => st.StartTime)
            .Description("Giờ bắt đầu ca");

        descriptor
            .Field(st => st.EndTime)
            .Description("Giờ kết thúc ca");

        descriptor
            .Field(st => st.WagePerHour)
            .Description("Lương theo giờ (VNĐ)");

        descriptor
            .Field(st => st.Attendances)
            .Description("Danh sách chấm công sử dụng ca này");
    }
}