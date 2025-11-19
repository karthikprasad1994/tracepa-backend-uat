using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace TracePA_Agent_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PythonRunnerController : ControllerBase
    {
        private readonly ILogger<PythonRunnerController> _logger;
        private readonly string _pythonScriptPath;

        public PythonRunnerController(ILogger<PythonRunnerController> logger)
        {
            _logger = logger;
            _pythonScriptPath = @"C:\TracePA_Agent\scripts\single_video_processor.py";
        }

        [HttpPost("process-video")]
        public async Task<IActionResult> ProcessVideo(
            IFormFile? videoFile = null,
            string targetAccent = "en-US") // default US accent
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (videoFile == null)
                    return BadRequest("Video file is required");

                if (!System.IO.File.Exists(_pythonScriptPath))
                    return NotFound($"Python script not found at {_pythonScriptPath}");

                // Validate file type
                var allowedExtensions = new[] { ".mp4", ".avi", ".mov", ".mkv" };
                var fileExtension = Path.GetExtension(videoFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest("Invalid file type. Supported: MP4, AVI, MOV, MKV");

                // Python executable
                string pythonExe = GetPythonExecutable();

                // Save uploaded file temporarily
                string videoPath;
                using (var tempFile = new TempFileHandler())
                {
                    videoPath = await tempFile.SaveUploadedFile(videoFile);

                    // Build Python arguments: video path + target accent
                    string args = $"\"{_pythonScriptPath}\" \"{videoPath}\" \"{targetAccent}\"";

                    // Execute Python script with 10 min timeout
                    var result = await ExecutePythonScript(pythonExe, args, TimeSpan.FromMinutes(10));

                    stopwatch.Stop();

                    _logger.LogInformation($"Video accent refinement completed in {stopwatch.Elapsed.TotalSeconds:F2}s");

                    return Ok(new
                    {
                        message = "Video processed successfully (accent refined)",
                        processing_time = $"{stopwatch.Elapsed.TotalSeconds:F2}s",
                        output = result.Output,
                        errors = result.Errors,
                        file_name = videoFile.FileName
                    });
                }
            }
            catch (TimeoutException ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Video processing timeout");
                return StatusCode(408, new { error = "Processing timeout. Video might be too long or complex." });
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error processing video");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        private string GetPythonExecutable()
        {
            string pythonExe = @"C:\Python37\python.exe";
            if (!System.IO.File.Exists(pythonExe))
            {
                pythonExe = "python";
                _logger.LogWarning("Using Python from PATH");
            }
            return pythonExe;
        }

        private async Task<(string Output, string Errors)> ExecutePythonScript(string pythonExe, string arguments, TimeSpan timeout)
        {
            var psi = new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputBuilder.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    errorBuilder.AppendLine(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            var processTask = Task.Run(() =>
            {
                process.WaitForExit();
                return (outputBuilder.ToString(), errorBuilder.ToString());
            });

            if (await Task.WhenAny(processTask, Task.Delay(timeout)) == processTask)
            {
                return await processTask;
            }
            else
            {
                process.Kill(true);
                throw new TimeoutException($"Python script execution timeout after {timeout.TotalMinutes} minutes");
            }
        }
    }

    // Temporary file handler
    public class TempFileHandler : IDisposable
    {
        private string _tempFilePath;

        public async Task<string> SaveUploadedFile(IFormFile file)
        {
            var uploadDir = @"C:\TracePA_Agent\incoming";
            Directory.CreateDirectory(uploadDir);

            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadDir, fileName);
            _tempFilePath = filePath;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filePath;
        }

        public void Dispose()
        {
            try
            {
                if (File.Exists(_tempFilePath))
                    File.Delete(_tempFilePath);
            }
            catch { }
        }
    }
}
