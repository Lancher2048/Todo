namespace Todo.Core.Mapping
{
    public class UserMap : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            //主键
            builder.HasKey(t => t.Id);
        }
    }
}
