namespace WebBimba.Models
{
    public class ErrorViewModel // ���� ��� ����������� �������
    {
        public string? RequestId { get; set; } // RequestId - ������������� ������

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId); // ���������� ������������� ������
    }
}
