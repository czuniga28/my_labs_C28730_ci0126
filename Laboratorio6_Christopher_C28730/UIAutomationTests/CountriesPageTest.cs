using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;

namespace UIAutomationTests
{
    [TestFixture]
    public class CountriesPageTest
    {
        private IWebDriver _driver;
        private WebDriverWait _wait;

        [SetUp]
        public void Setup()
        {
            // Configurar ChromeDriver
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--start-maximized");
            chromeOptions.AddArgument("--disable-notifications");
            
            _driver = new ChromeDriver(chromeOptions);
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void Test_NavigateToAddCountryAndCreate()
        {
            try
            {
                // Arrange
                var baseUrl = "http://localhost:8080";
                
                // Act - Navegar a la página principal
                _driver.Navigate().GoToUrl(baseUrl);

                // Assert 1: Verificar que la página principal carga correctamente
                Assert.That(_driver.Title, Is.Not.Empty, "La página debería tener un título");
                
                var pageTitle = _driver.FindElement(By.TagName("h1"));
                Assert.That(pageTitle.Text, Is.EqualTo("Lista de países"), 
                    "El título de la página debería ser 'Lista de países'");

                // Act - Hacer clic en el botón "Agregar país"
                var addButton = _driver.FindElement(By.XPath("//a[contains(@href, '/country')]"));
                addButton.Click();

                // Esperar a que cargue la nueva página
                _wait.Until(driver => 
                    driver.Url.Contains("country") || 
                    driver.FindElements(By.TagName("form")).Count > 0);

                // Assert 2: Verificar que estamos en la página de agregar país
                Assert.That(_driver.Url, Contains.Substring("country"), 
                    "Debería estar en la página de agregar país");

                // Buscar elementos del formulario (ajusta estos selectores según tu formulario real)
                var formElements = _driver.FindElements(By.TagName("input"));
                var submitButton = _driver.FindElement(By.XPath("//button[@type='submit']"));

                // Si encontramos campos del formulario, llenarlos
                if (formElements.Count >= 3)
                {
                    // Llenar nombre del país
                    formElements[0].Clear();
                    formElements[0].SendKeys("Brasil");

                    // Llenar continente
                    formElements[1].Clear();
                    formElements[1].SendKeys("América");

                    // Llenar idioma
                    formElements[2].Clear();
                    formElements[2].SendKeys("Portugués");

                    // Enviar formulario
                    submitButton.Click();

                    // Esperar redirección o mensaje de confirmación
                    _wait.Until(driver => 
                        driver.Url.Contains("list") || 
                        driver.FindElements(By.ClassName("alert-success")).Count > 0 ||
                        driver.FindElements(By.XPath("//*[contains(text(), 'éxito')]")).Count > 0);

                    // Assert 3: Verificar mensaje de confirmación o redirección
                    var successElements = _driver.FindElements(By.XPath("//*[contains(text(), 'éxito')]"));
                    if (successElements.Count == 0)
                    {
                        successElements = _driver.FindElements(By.ClassName("alert-success"));
                    }
                    
                    // Verificar que estamos de vuelta en la lista o hay mensaje de éxito
                    Assert.That(_driver.Url.Contains("list") || successElements.Count > 0, 
                        "Debería redirigir a la lista o mostrar mensaje de éxito");
                }
                else
                {
                    // Si no encontramos el formulario, al menos verificar que estamos en la página correcta
                    var form = _driver.FindElement(By.TagName("form"));
                    Assert.That(form, Is.Not.Null, "Debería haber un formulario en la página");
                }

                // Assert 4: Verificar que podemos volver a la lista principal
                _driver.Navigate().GoToUrl(baseUrl);
                var mainTitle = _driver.FindElement(By.TagName("h1"));
                Assert.That(mainTitle.Text, Is.EqualTo("Lista de países"), 
                    "Debería poder volver a la lista principal");

                // Assert 5: Verificar que la tabla de países existe y tiene datos
                var table = _driver.FindElement(By.ClassName("table"));
                var tableRows = table.FindElements(By.TagName("tr"));
                Assert.That(tableRows.Count, Is.GreaterThan(1), 
                    "La tabla debería tener al menos una fila de datos");

                Console.WriteLine("✅ Todas las verificaciones pasaron correctamente");
            }
            catch (Exception ex)
            {
                // Capturar screenshot en caso de error
                var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                screenshot.SaveAsFile("error_screenshot.png");
                throw new Exception($"Prueba falló: {ex.Message}");
            }
        }

        [Test]
        public void Test_DeleteCountryFunctionality()
        {
            try
            {
                // Arrange
                _driver.Navigate().GoToUrl("http://localhost:8080");
                
                // Esperar a que la página cargue completamente y Vue haya renderizado
                _wait.Until(driver => 
                {
                    var tables = driver.FindElements(By.ClassName("table"));
                    if (tables.Count == 0) return false;
                    var tbody = tables[0].FindElements(By.TagName("tbody"));
                    return tbody.Count > 0 && tbody[0].FindElements(By.TagName("tr")).Count > 0;
                });

                // Act - Buscar y hacer clic en el primer botón Eliminar
                var deleteButtons = _driver.FindElements(By.XPath("//button[contains(text(), 'Eliminar')]"));
                
                if (deleteButtons.Count == 0)
                {
                    // Si no hay botones de eliminar, buscar por otros selectores comunes
                    deleteButtons = _driver.FindElements(By.XPath("//a[contains(text(), 'Eliminar')]"));
                }
                
                if (deleteButtons.Count == 0)
                {
                    // Buscar por clase o atributos comunes
                    deleteButtons = _driver.FindElements(By.CssSelector("button.btn-danger, a.btn-danger, button[class*='delete'], a[class*='delete']"));
                }
                
                Assert.That(deleteButtons.Count, Is.GreaterThan(0), 
                    "Debería haber al menos un botón de eliminar en la página");

                // Guardar el número inicial de filas en tbody (solo filas de datos, sin header)
                var table = _driver.FindElement(By.ClassName("table"));
                var tbody = table.FindElement(By.TagName("tbody"));
                var initialDataRows = tbody.FindElements(By.TagName("tr")).Count;
                Assert.That(initialDataRows, Is.GreaterThan(0), 
                    "Debería haber al menos una fila de datos para eliminar");

                // Scroll al botón para asegurar que sea visible y clickeable
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", deleteButtons[0]);
                System.Threading.Thread.Sleep(200); // Pequeña pausa para asegurar que el scroll se complete

                // Hacer clic en el primer botón Eliminar usando JavaScript para evitar problemas de visibilidad
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", deleteButtons[0]);

                // Esperar a que Vue procese la eliminación y actualice el DOM
                // Esperamos a que el número de filas en tbody disminuya
                _wait.Until(driver =>
                {
                    try
                    {
                        var currentTable = driver.FindElement(By.ClassName("table"));
                        var currentTbody = currentTable.FindElement(By.TagName("tbody"));
                        var currentDataRows = currentTbody.FindElements(By.TagName("tr")).Count;
                        return currentDataRows < initialDataRows;
                    }
                    catch (StaleElementReferenceException)
                    {
                        // El elemento puede estar obsoleto durante la actualización de Vue, intentar de nuevo
                        return false;
                    }
                    catch (NoSuchElementException)
                    {
                        // La tabla puede estar siendo actualizada, intentar de nuevo
                        return false;
                    }
                });

                // Assert: Verificar que se eliminó una fila
                var updatedTable = _driver.FindElement(By.ClassName("table"));
                var updatedTbody = updatedTable.FindElement(By.TagName("tbody"));
                var updatedDataRows = updatedTbody.FindElements(By.TagName("tr")).Count;
                
                Assert.That(updatedDataRows, Is.LessThan(initialDataRows), 
                    $"Debería haber una fila menos después de eliminar. Filas iniciales: {initialDataRows}, Filas actuales: {updatedDataRows}");

                Console.WriteLine("✅ La funcionalidad de eliminar país funcionó correctamente");
            }
            catch (Exception ex)
            {
                // Capturar screenshot en caso de error
                try
                {
                    var screenshot = ((ITakesScreenshot)_driver).GetScreenshot();
                    screenshot.SaveAsFile("error_delete_screenshot.png");
                }
                catch { }
                throw new Exception($"Prueba de eliminación falló: {ex.Message}", ex);
            }
        }

        [TearDown]
        public void Teardown()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}