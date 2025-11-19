using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using Reqnroll;

namespace BookingAPI.StepDefinitions
{
    [Binding]
    public sealed class Hooks
    {
        private static ExtentReports? _extent;
        private static ExtentTest? _feature;
        private static ExtentTest? _scenario;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var resultsDir = Environment.GetEnvironmentVariable("TEST_RESULTS_DIR") ?? "/app/TestResults";
            
            if (!Path.IsPathRooted(resultsDir))
            {
                var baseDir = AppDomain.CurrentDomain.BaseDirectory;
                var projectRoot = Path.GetFullPath(Path.Combine(baseDir, @"..\..\.."));
                resultsDir = Path.Combine(projectRoot, "TestResults");
            }
            
            Directory.CreateDirectory(resultsDir);
            
            var reporter = new ExtentSparkReporter(Path.Combine(resultsDir, "TestReport.html"));
            
            reporter.Config.DocumentTitle = "Booking API - Test Execution Report";
            reporter.Config.ReportName = "Restful-Booker API Test Results";
            reporter.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;
            
            _extent = new ExtentReports();
            _extent.AttachReporter(reporter);
            
            _extent.AddSystemInfo("Application", "Restful-Booker API");
            _extent.AddSystemInfo("Environment", "Production");
            _extent.AddSystemInfo("Base URL", BookingAPI.Helpers.Config.BaseUrl);
            _extent.AddSystemInfo("Test Framework", "Reqnroll (BDD)");
            _extent.AddSystemInfo("Assertion Library", "NUnit");
            _extent.AddSystemInfo("HTTP Client", "RestSharp");
            _extent.AddSystemInfo("Executed By", Environment.UserName);
            _extent.AddSystemInfo("Machine", Environment.MachineName);
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _extent?.Flush();
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            _feature = _extent?.CreateTest<Feature>(featureContext.FeatureInfo.Title);
        }

        [BeforeScenario]
        public async Task BeforeScenario(ScenarioContext context)
        {
            _scenario = _feature?.CreateNode<Scenario>(context.ScenarioInfo.Title);
            await Task.Delay(3000);
        }

        [AfterStep]
        public void AfterStep(ScenarioContext context)
        {
            if (_scenario == null) return;

            var stepInfo = context.StepContext.StepInfo;
            
            if (context.TestError != null)
            {
                var stepNode = _scenario.CreateNode<Then>(stepInfo.Text);
                stepNode.Fail($"<b>Error:</b> {context.TestError.Message}");
            }
        }

        [AfterScenario]
        public void AfterScenario(ScenarioContext context)
        {
            if (_scenario == null) return;
            
            if (context.ContainsKey("ApiRequest"))
            {
                var request = context["ApiRequest"].ToString();
                var requestNode = _scenario.CreateNode<Given>("API Request Details");
                requestNode.Pass($"<pre>{request}</pre>");
            }
            
            if (context.ContainsKey("ApiResponse"))
            {
                var response = context["ApiResponse"].ToString();
                var responseNode = _scenario.CreateNode<Then>("API Response Details");
                responseNode.Pass($"<pre>{response}</pre>");
            }
            
            if (context.TestError != null)
            {
                _scenario.Fail($"<pre>{context.TestError.Message}</pre>");
                
                if (context.TestError.StackTrace != null)
                {
                    _scenario.Info($"<pre>{context.TestError.StackTrace}</pre>");
                }
            }
            else
            {
                _scenario.Pass("Test Passed");
            }
        }
    }
}
