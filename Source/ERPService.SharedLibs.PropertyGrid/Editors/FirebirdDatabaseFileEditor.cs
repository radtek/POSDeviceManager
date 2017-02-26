namespace ERPService.SharedLibs.PropertyGrid.Editors
{
    /// <summary>
    /// �������� ��� �������-���� ������ ��� ������ Firebird, Interbase
    /// </summary>
    public class FirebirdDatabaseFileEditor : CustomFileNameEditor
    {
        /// <summary>
        /// �������������� ���� ������
        /// </summary>
        protected override FileType[] SupportedFileTypes
        {
            get
            {
                return new FileType[] {
                    new FileType("���� ������ Firebird", "fdb"),
                    new FileType("���� ������ Interbase", "gdb"),
                    new FileType("���� ������ Interbase 7", "ib"),
                };
            }
        }
    }
}
