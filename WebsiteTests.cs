using NUnit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Runtime.CompilerServices;

namespace TestProject3
{
    public class Websites_tests
    {
        ChromeDriver driver;  

        private const string HomeUrl = "https://the-internet.herokuapp.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [Test]
        public void CheckBoxes()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/checkboxes");
            var checkboxes = driver.FindElements(By.CssSelector("#checkboxes input"));

            var checkbox1 = checkboxes[0];
            var checkbox2 = checkboxes[1];
            
            Assert.That(checkbox1.Selected, Is.False);
            Assert.That(checkbox2.Selected, Is.True);

            if (!checkbox1.Selected)
            {
                checkbox1.Click();
            }

            if (checkbox2.Selected)
            {
                checkbox2.Click();
            }

            Assert.That(checkbox1.Selected, Is.True);
            Assert.That(checkbox2.Selected, Is.False);
        }

        [Test]
        public void AddRemoveElements()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/add_remove_elements/");
            var addButton = driver.FindElement(By.CssSelector("button[onclick='addElement()']"));
            addButton.Click();
            addButton.Click();
            var deleteButtons = driver.FindElements(By.CssSelector(".added-manually"));
            Assert.That(deleteButtons.Count, Is.EqualTo(2));
            deleteButtons[0].Click();
            deleteButtons = driver.FindElements(By.CssSelector(".added-manually"));
            Assert.That(deleteButtons.Count, Is.EqualTo(1));
        }

        [Test]
        public void DropDown()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/dropdown");
            var dropdown = new SelectElement(driver.FindElement(By.Id("dropdown")));
            dropdown.SelectByValue("1");
            Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 1"));
            dropdown.SelectByText("Option 2");
            Assert.That(dropdown.SelectedOption.Text, Is.EqualTo("Option 2"));
        }

        [Test]
        public void Inputs()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/inputs");
            var inputField = driver.FindElement(By.TagName("input"));
            inputField.Clear();
            inputField.SendKeys("56");
            Assert.That(inputField.GetAttribute("value"), Is.EqualTo("56"));
            inputField.Clear();
            inputField.SendKeys("-21");
            Assert.That(inputField.GetAttribute("value"), Is.EqualTo("-21"));
        }

        [Test]
        public void StatusCodes()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/status_codes");
            var links = driver.FindElements(By.CssSelector(".example ul li a"));
            foreach (var link in links)
            {
                var statusCode = link.Text;
                link.Click();
                Assert.That(driver.Url, Does.Contain($"status_codes/{statusCode}"));
                driver.Navigate().Back();
            }
        }

        [Test]
        public void NestedFrames()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/nested_frames");
            driver.SwitchTo().Frame("frame-top");
            driver.SwitchTo().Frame("frame-left");
            var leftText = driver.FindElement(By.TagName("body")).Text;
            Assert.That(leftText, Is.EqualTo("LEFT"));
            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame("frame-middle");
            var middleText = driver.FindElement(By.Id("content")).Text;
            Assert.That(middleText, Is.EqualTo("MIDDLE"));
            driver.SwitchTo().ParentFrame();
            driver.SwitchTo().Frame("frame-right");
            var rightText = driver.FindElement(By.TagName("body")).Text;
            Assert.That(rightText, Is.EqualTo("RIGHT"));
            driver.SwitchTo().DefaultContent();
            driver.SwitchTo().Frame("frame-bottom");
            var bottomText = driver.FindElement(By.TagName("body")).Text;
            Assert.That(bottomText, Is.EqualTo("BOTTOM"));
        }

        [Test]
        public void FormAuthentication_InvalidPassword_ShowsErrorMessage()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/login");

            driver.FindElement(By.Id("username")).SendKeys("tomsmith");
            driver.FindElement(By.Id("password")).SendKeys("IdkThePassword!");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var errorMessage = driver.FindElement(By.Id("flash"));
            Assert.That(errorMessage.Text, Does.Contain("Your password is invalid!"));
        }

        [Test]
        public void FormAuthentication_InvalidUsername_ShowsErrorMessage()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/login");

            driver.FindElement(By.Id("username")).SendKeys("tom");
            driver.FindElement(By.Id("password")).SendKeys("IdkThePassword!");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var errorMessage = driver.FindElement(By.Id("flash"));
            Assert.That(errorMessage.Text, Does.Contain("Your username is invalid!"));
        }

        [Test]
        public void FormAuthentication_ValidCredentials_LogsInAndLogsOutSuccessfully()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/login");

            driver.FindElement(By.Id("username")).SendKeys("tomsmith");
            driver.FindElement(By.Id("password")).SendKeys("SuperSecretPassword!");
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            var successMessage = driver.FindElement(By.Id("flash"));
            Assert.That(successMessage.Text, Does.Contain("You logged into a secure area!"));

            driver.FindElement(By.CssSelector("a.button.secondary[href='/logout']")).Click();

            var logoutMessage = driver.FindElement(By.Id("flash"));
            Assert.That(logoutMessage.Text, Does.Contain("You logged out of the secure area!"));
        }

        [Test]
        public void DynamicContent_RefreshChangesContent()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/dynamic_content");
            var textBeforeRefresh = driver.FindElement(By.CssSelector(".large-10.columns")).Text;
            var imgBeforeRefresh = driver.FindElement(By.CssSelector(".large-2.columns img"));
            
            driver.Navigate().Refresh();
            var textAfterRefresh = driver.FindElement(By.CssSelector(".large-10.columns")).Text;
            var imgAfterRefresh = driver.FindElement(By.CssSelector(".large-2.columns img"));
            
            Assert.That(textBeforeRefresh, Is.Not.EqualTo(textAfterRefresh));
            Assert.That(imgBeforeRefresh, Is.Not.EqualTo(imgAfterRefresh));
        }

        [Test]
        public void DisappearingElements()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/disappearing_elements");

            var menuButtons = driver.FindElements(By.CssSelector(".example ul li"));
            Assert.That(menuButtons.Count, Is.GreaterThanOrEqualTo(4));

            var homeButton = driver.FindElement(By.LinkText("Home"));
            Assert.That(homeButton.Displayed, Is.True);

            bool isGalleryFound = false;
            int maxAttempts = 10;

            for (int i = 0; i < maxAttempts; i++)
            {
                var galleryElements = driver.FindElements(By.LinkText("Gallery"));

                if (galleryElements.Count > 0 && galleryElements[0].Displayed)
                {
                    isGalleryFound = true;
                    break;
                }

                driver.Navigate().Refresh();
            }

            Assert.That(isGalleryFound, Is.True);
        }

        [Test]
        public void Hover()
        {
            driver.Navigate().GoToUrl($"{HomeUrl}/hovers");
            var figures = driver.FindElements(By.CssSelector(".figure"));
            for (int i = 0; i < figures.Count; i++)
            {
                var figure = figures[i];
                var action = new OpenQA.Selenium.Interactions.Actions(driver);
                action.MoveToElement(figure).Perform();
                var caption = figure.FindElement(By.CssSelector(".figcaption"));
                Assert.That(caption.Displayed, Is.True);
            }
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}