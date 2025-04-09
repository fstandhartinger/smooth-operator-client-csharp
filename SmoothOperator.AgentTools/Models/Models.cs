using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection; // Added for DescriptionAttribute

namespace SmoothOperator.AgentTools.Models
{
    /// <summary>
    /// Specifies the mechanism to use for AI-based UI element interaction.
    /// </summary>
    public enum MechanismType
    {
        [Description("screengrasp2")]
        ScreenGrasp2,

        [Description("screengrasp2-low")]
        ScreenGrasp2Low,

        [Description("screengrasp-medium")]
        ScreenGraspMedium,

        [Description("screengrasp-high")]
        ScreenGraspHigh,

        [Description("llabs")]
        LLabs,

        [Description("anthropic-computer-use")]
        AnthropicComputerUse,

        [Description("openai-computer-use")]
        OpenAIComputerUse,

        [Description("qwen25-vl-72b")]
        Qwen25Vl72b
    }

    /// <summary>
    /// Helper methods for enums.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the string value from the DescriptionAttribute of an enum value.
        /// </summary>
        /// <param name="value">The enum value.</param>
        /// <returns>The description string, or the enum's name if no description is found.</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }
    }

    /// <summary>
    /// Response from the screenshot endpoint
    /// </summary>
    public class ScreenshotResponse
    {
        public bool Success { get; set; } = true;
        public string ImageBase64 { get; set; }
        public byte[] ImageBytes => Convert.FromBase64String(ImageBase64);
        public string ImageMimeType => "image/jpeg";
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the screenshot response.
        /// </summary>
        public override string ToString() => Message;

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// Represents a point on the screen with X and Y coordinates
    /// </summary>
    public class Point
    {
        /// <summary>
        /// X coordinate
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y coordinate
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Returns a string representation of the point coordinates.
        /// </summary>
        public override string ToString() => $"({X}, {Y})";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// Information about a Chrome browser tab
    /// </summary>
    public class ChromeTab
    {
        /// <summary>
        /// Tab ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Tab title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tab URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Whether the tab is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the Chrome tab information.
        /// </summary>
        public override string ToString() => $"{(IsActive ? "[Active] " : "")}{Title ?? Url} ({Id})";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// Response from the ScreenGrasp2 endpoint (find-ui-element-by-description)
    /// Inherits Success and Message from ActionResponse.
    /// </summary>
    public class ScreenGrasp2Response : ActionResponse
    {
        /// <summary>
        /// X coordinate of the found element (if applicable)
        /// </summary>
        public int? X { get; set; }

        /// <summary>
        /// Y coordinate of the found element (if applicable)
        /// </summary>
        public int? Y { get; set; }

        /// <summary>
        /// Status message from the underlying service
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the ScreenGrasp2 response.
        /// </summary>
        public override string ToString() => $"ScreenGrasp2: {(Success ? "Success" : "Failed")}{(X.HasValue && Y.HasValue ? $" at ({X},{Y})" : "")}. Status: {Status ?? Message}";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// Generic response for action endpoints
    /// </summary>
    public class ActionResponse
    {
        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Message describing the result
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Additional result data data (if applicable)
        /// </summary>
        public string ResultValue { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the action response.
        /// </summary>
        public override string ToString() => $"Action: {(Success ? "Success" : "Failed")}. {Message}";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// Detailed information about a Chrome tab
    /// </summary>
    public class ChromeTabDetails
    {
        /// <summary>
        /// Tab title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tab URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Page content
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// List of elements on the page
        /// </summary>
        public List<Dictionary<string, object>> Elements { get; set; }

        /// <summary>
        /// Summary of the page content
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the Chrome tab details.
        /// </summary>
        public override string ToString() => $"Tab Details: {Title ?? Url}. Summary: {Summary?.Substring(0, Math.Min(Summary.Length, 50))}...";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class ChromeScriptResponse : ActionResponse
    {
        public string Result { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the Chrome script response.
        /// </summary>
        public override string ToString() => $"Script Result: {(Success ? "Success" : "Failed")}. {(Result != null ? $"Result: {Result.Substring(0, Math.Min(Result.Length, 50))}..." : Message)}";
    }

    public class CSharpCodeResponse : ActionResponse
    {
        public string Result { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the C# code response.
        /// </summary>
        public override string ToString() => $"C# Result: {(Success ? "Success" : "Failed")}. {(Result != null ? $"Result: {Result.Substring(0, Math.Min(Result.Length, 50))}..." : Message)}";
    }

    public class OverviewResponse
    {
        public List<WindowInfoDTO> Windows { get; set; }

        public List<ChromeOverview> ChromeInstances { get; set; }

        public FocusInformation FocusInfo { get; set; }

        public List<TaskbarIconDTO> TopPinnedTaskbarIcons { get; set; }

        public List<DesktopIconDTO> TopDesktopIcons { get; set; }

        public List<InstalledProgramDTO> TopInstalledPrograms { get; set; }

        public string ImportantNote { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the overview response.
        /// </summary>
        public override string ToString() => $"Overview: {Windows?.Count ?? 0} Windows, {ChromeInstances?.Count ?? 0} Chrome Instances. Focus: {FocusInfo?.ToString() ?? "N/A"}";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class InstalledProgramDTO
    {
        public string Name { get; set; }
        public string ExecutablePath { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(ExecutablePath)}: {ExecutablePath}";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class DesktopIconDTO
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Path)}: {Path}";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    }

    public class TaskbarIconDTO
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Path)}: {Path}";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    }


    public class ChromeElementInfo
    {
        public string SmoothOpId { get; set; }

        public string TagName { get; set; }

        public string CssSelector { get; set; }

        public string InnerText { get; set; }

        public bool? IsVisible { get; set; }

        public double Score { get; set; }

        public string Role { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string ClassName { get; set; }

        public string Semantic { get; set; }

        public string DataAttributes { get; set; }

        public string TruncatedHtml { get; set; }

        public int[] BoundingRect { get; set; }

        public Point CenterPoint { get; set; }

        public override string ToString()
        {
            var displayText = InnerText ?? Value ?? "";
            var roleInfo = !string.IsNullOrEmpty(Role) ? $" role={Role}" : "";
            var typeInfo = !string.IsNullOrEmpty(Type) ? $" type={Type}" : "";
            var visibilityInfo = IsVisible != null && !IsVisible.Value ? " (hidden)" : "";
            var semanticInfo = !string.IsNullOrEmpty(Semantic) ? $" semantic={Semantic}" : "";
            return $"<{TagName}{roleInfo}{typeInfo}>{displayText}{visibilityInfo}{semanticInfo} (score: {Score})";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    }

    public class FocusInformation
    {
        public ControlDTO FocusedElement { get; set; }

        public WindowInfoDTO FocusedElementParentWindow { get; set; }

        public List<ControlDTO> SomeOtherElementsInSameWindowThatMightBeRelevant { get; set; }

        public List<ChromeElementInfo> CurrentChromeTabMostRelevantElements { get; set; }

        public bool IsChrome { get; set; }

        public string Note { get; set; }

        public override string ToString()
        {
            return FocusedElement?.ToString() ?? FocusedElementParentWindow?.ToString() ?? "No focused element";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    }

    public class WindowInfoDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ExecutablePath { get; set; }
        public bool? IsForeground { get; set; } //null == false
        public string ProcessName { get; set; }
        public bool? IsMinimized { get; set; }
        public WindowDetailResponse DetailInfos { get; set; }

        public override string ToString()
        {
            return $"{(IsForeground != null && IsForeground.Value ? "[FOREGROUND] " : "")}{Title ?? ProcessName ?? ExecutablePath ?? Id}";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    }

    public class WindowDetailResponse
    {
        public WindowDetailInfosDTO Details { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the window detail response.
        /// </summary>
        public override string ToString() => $"Window Detail: {(Details != null ? "Available" : "Not Available")}. {Message}";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });

    }

    public class WindowDetailInfosDTO
    {
        public string Note { get; set; }
        public WindowInfoDTO Window { get; }
        public ControlDTO UserInterfaceElements { get; }

        public WindowDetailInfosDTO(WindowInfoDTO window, ControlDTO userInterfaceElements)
        {
            Window = window;
            UserInterfaceElements = userInterfaceElements;
        }

        public override string ToString()
        {
            return $"{{ Window = {Window}, UserInterfaceElements = {UserInterfaceElements} }}";
        }

        public override bool Equals(object value)
        {
            return value is WindowDetailInfosDTO other && EqualityComparer<WindowInfoDTO>.Default.Equals(other.Window, Window) && EqualityComparer<ControlDTO>.Default.Equals(other.UserInterfaceElements, UserInterfaceElements);
        }

        public override int GetHashCode()
        {
            var hash = 0x7a2f0b42;
            hash = (-1521134295 * hash) + EqualityComparer<WindowInfoDTO>.Default.GetHashCode(Window);
            return (-1521134295 * hash) + EqualityComparer<ControlDTO>.Default.GetHashCode(UserInterfaceElements);
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class ControlDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        
        public string ControlType { get; set; }

        public bool? SupportsSetValue { get; set; }
        public bool? SupportsInvoke { get; set; }
        public string CurrentValue { get; set; }

        public List<ControlDTO> Children { get; set; }

        public ControlDTO Parent { get; set; }
        [JsonIgnore]
        public IEnumerable<ControlDTO> ChildrenRecursive
        {
            get
            {
                if (Children == null)
                {
                    yield break;
                }

                foreach (var child in Children)
                {
                    yield return child;
                    if (child.Children == null) continue;
                    foreach (var subChild in child.ChildrenRecursive)
                        yield return subChild;
                }
            }
        }

        private List<ControlDTO> allChildrenRecursive;
        [JsonIgnore]
        public List<ControlDTO> AllChildrenRecursive =>
            allChildrenRecursive ??= new List<ControlDTO>(ChildrenRecursive.ToList());
        [JsonIgnore]
        public IEnumerable<ControlDTO> ParentsRecursive
        {
            get
            {
                var x = this;

                while (x.Parent != null)
                {
                    yield return x.Parent;
                    x = x.Parent;
                }
            }
        }
        [JsonIgnore]
        public ControlDTO ParentWindow => ParentsRecursive.FirstOrDefault(x => x.ControlType == "Window");

        private List<ControlDTO> allParentsRecursive;
        [JsonIgnore]
        public List<ControlDTO> AllParentsRecursive =>
            allParentsRecursive ??= new List<ControlDTO>(ParentsRecursive.ToList());

        public IEnumerable<ControlDTO> GetChildrenRecursive(bool includeSelf = false)
        {
            if (includeSelf)
                yield return this;
            if (Children == null) yield break;
            foreach (ControlDTO child in Children)
            {
                yield return child;
                var grandChildren = child.GetChildrenRecursive();
                foreach (var grandChild in grandChildren)
                    yield return grandChild;
            }
        }

        public override string ToString()
        {
            var valueInfo = !string.IsNullOrEmpty(CurrentValue) ? $" Value='{CurrentValue}'" : "";
            return $"{ControlType ?? "Control"} '{Name ?? Id}'{valueInfo}";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class ChromeOverview
    {
        public string InstanceId { get; set; }
        public List<TabData> Tabs { get; set; }
        public DateTime LastUpdate { get; set; }

        public override string ToString()
        {
            return $"{InstanceId}: {Tabs?.Count ?? 0} Tabs";
        }

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class TabData
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public bool? IsActive { get; set; }

        public string Html { get; set; }
        public string Text { get; set; }
        public string IdString { get; set; }

        public int TabNr { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the tab data.
        /// </summary>
        public override string ToString() => $"Tab {TabNr}{(IsActive == true ? " [Active]" : "")}: {Url ?? IdString}";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public class SimpleResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string InternalMessage { get; set; }

        /// <summary>
        /// Returns a string representation summarizing the simple response.
        /// </summary>
        public override string ToString() => $"Response: {Message ?? InternalMessage ?? "OK"}";

        /// <summary>
        /// Returns a JSON representation of the current object, for example to pass it to a LLM in order to decide on the next steps;
        /// </summary>
        /// <returns></returns>
        public string ToJsonString() => JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
}
