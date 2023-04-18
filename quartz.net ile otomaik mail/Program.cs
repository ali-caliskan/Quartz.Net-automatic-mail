

//form içine
private IScheduler _scheduler;

//public table içine 
InitializeScheduler();

//static string kimeYollanacak;
static List<string> kimeYollanacakList = new List<string>();
private void InitializeScheduler()
{
    // Quartz.NET Scheduler'ýný oluþtur
    var schedulerFactory = new StdSchedulerFactory();
    _scheduler = schedulerFactory.GetScheduler().Result;
    _scheduler.Start().Wait();

    // Mail gönderme görevini tanýmla
    var mailJob = JobBuilder.Create<MailJob>().Build();

    // Mail gönderme trigger'ýný oluþtur
    var mailTrigger = TriggerBuilder.Create();

    mailTrigger.WithSimpleSchedule(s =>
            s.WithInterval(TimeSpan.FromHours(24 / (double)ReportDailyNum.Value))

             // s.WithIntervalInHours(24 / (int)ReportDailyNum.Value)
             .RepeatForever());

    // Numeric up down deðerine göre mail gönderme sayýsý belirle
    int mailCount = (int)ReportDailyNum.Value;


    var trigger = mailTrigger.Build();

    // Mail gönderme görevini trigger ile scheduler'a ekle
    _scheduler.ScheduleJob(mailJob, trigger).Wait();


}
public class MailJob : IJob
{

    bool enableSSL = false;


    public async Task Execute(IJobExecutionContext context)
    {

        // Mail gönderme iþlemi
        // string to = kimeYollanacak;
        string from = "testhesabi9@outlook.com";
        string subject = "MYSÝLO";
        string body = "This is a test mail. Sent on: " + DateTime.Now.ToString();

        if (secilenDeger == "SSL")
        {
            enableSSL = true;

        }
        else if (secilenDeger == "SSL/TLS")
        {
            enableSSL = false;
        }
        else
        {
            enableSSL = false;
        }
        foreach (string to in kimeYollanacakList)
        {
            var message = new MailMessage(from, to, subject, body)

            {
                //özel karakter ve türkçe karakter için.
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8

            };


            var client = new SmtpClient("smtp.outlook.com", 587)
            {
                Credentials = new NetworkCredential("testhesabi9@outlook.com", "Deneme89."),

                EnableSsl = enableSSL

            };
            await client.SendMailAsync(message);
        }


    }

}



static string secilenDeger;
//btnsendmail 
private async void button3_Click(object sender, EventArgs e)
{

    Console.WriteLine(secilenDeger);
    // Console.WriteLine(kimeYollanacak);
    kimeYollanacakList = Receptienstxt.Text.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();
    //kimeYollanacakList = Receptienstxt.Text.Split(';').ToList();
    //kimeYollanacak = Receptienstxt.Text.ToString();
    secilenDeger = SsslComboBox.SelectedItem.ToString();
    int mailCount = (int)ReportDailyNum.Value;
    //EmailEnableCheckBox seçili ise mail gönderiliyor.
    if (EmailEnableCheckBox.Checked)
    {
        InitializeScheduler();

    }
    else
    {
        MessageBox.Show("Please select the 'Send Mail' checkbox to send the email.");
    }



}