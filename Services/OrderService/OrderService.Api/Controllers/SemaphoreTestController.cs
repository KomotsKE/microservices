using Microsoft.AspNetCore.Mvc;
using Corelib.Distributed.interfaces;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("orderservice/api/[controller]")]
    public class SemaphoreTestController : ControllerBase
    {
        private readonly IDistributedSemaphore _semaphore;

        public SemaphoreTestController(IDistributedSemaphore semaphore)
        {
            _semaphore = semaphore;
        }

        /// <summary>
        /// Попытаться захватить семафор и удерживать заданное количество секунд.
        /// Возвращает 200 если захват успешен, 423 если не удалось захватить.
        /// </summary>
        /// <param name="holdSeconds">Сколько секунд удерживать захват (по умолчанию 5)</param>
        /// <param name="acquireTimeoutSeconds">Таймаут попытки захвата в секундах (по умолчанию 3)</param>
        [HttpPost("test")]
        public async Task<IActionResult> Test(int holdSeconds = 5, int acquireTimeoutSeconds = 3)
        {
            var timeout = TimeSpan.FromSeconds(acquireTimeoutSeconds);

            var handle = await _semaphore.TryAcquireAsync(timeout);
            if (handle == null)
            {
                return StatusCode(423, new { acquired = false, reason = "timeout" });
            }

            await using (handle)
            {
                // Удерживаем захват указанное время
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(holdSeconds));
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }

                return Ok(new { acquired = true, holdSeconds, acquireTimeoutSeconds });
            }
        }
    }
}
