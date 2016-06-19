namespace CrashReport.Client
{
	public interface ISender
	{
		void Send(Message message);
	}
}