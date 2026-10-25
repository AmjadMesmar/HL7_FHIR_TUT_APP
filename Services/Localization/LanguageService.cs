using System;
using System.Collections.Generic;

namespace Hl7FhirWorkbench.Services.Localization;

public class LanguageService
{
    public string CurrentLanguage { get; private set; } = "en";
    public bool IsRtl => CurrentLanguage == "ar";
    public string Direction => IsRtl ? "rtl" : "ltr";

    public event Action? OnLanguageChanged;

    private readonly Dictionary<string, (string en, string ar)> _translations = new(StringComparer.OrdinalIgnoreCase)
    {
        // Global / Header
        ["app.title"] = ("HL7 v2 & FHIR Teaching Workbench", "منصة تعليم معايير HL7 v2 و FHIR التفاعلية"),
        ["app.subtitle"] = ("Interactive Healthcare Interoperability Teaching Workbench", "بوابة تفاعلية لتعليم وتدريب تكامل الأنظمة الصحية"),
        ["app.jargon_glossary"] = ("Jargon Glossary", "معجم المصطلحات الطبية"),
        ["app.lang.switch"] = ("العربية", "English"),
        ["app.live_badge"] = ("Live In-Memory", "حي في الذاكرة"),
        
        // Navigation Sections
        ["nav.curriculum"] = ("Curriculum & Docs", "المناهج والوثائق التعليمية"),
        ["nav.labs"] = ("Interactive Labs", "المختبرات التفاعلية"),
        ["nav.tools"] = ("Engineering & QA Tools", "أدوات الهندسة والجودة"),
        ["nav.regional_training"] = ("Regional & Academy", "المعايير الإقليمية والشهادات"),
        
        // Navigation Items
        ["nav.overview"] = ("Overview", "نظرة عامة والمسار"),
        ["nav.master_guide"] = ("Master Guide", "الدليل الشامل"),
        ["nav.living_docs"] = ("Living Docs", "الوثائق الحية"),
        ["nav.interop_bridge"] = ("Interop Bridge", "جسر التحويل البيني"),
        ["nav.hl7_inspector"] = ("HL7 v2 & Linter", "فاحص ومحلل HL7 v2"),
        ["nav.fhir_explorer"] = ("FHIR Explorer", "مستكشف موارد FHIR"),
        ["nav.rest_playground"] = ("REST Playground", "مختبر استعلامات REST"),
        ["nav.hospital_flow"] = ("Hospital Flow Sim", "محاكي تدفق رسائل المستشفى"),
        ["nav.data_factory"] = ("Data Factory", "مصنع البيانات السريرية"),
        ["nav.mllp_sim"] = ("MLLP & ACK Sim", "محاكي MLLP وإشعارات ACK"),
        ["nav.terminology"] = ("Terminology Lookup", "مستعرض المصطلحات LOINC/ICD"),
        ["nav.code_recipes"] = ("C# Code Recipes", "وصفات برمجية C#"),
        ["nav.test_generator"] = ("xUnit Test Gen", "مولد اختبارات xUnit"),
        ["nav.war_room"] = ("War Room (Bugs)", "غرفة العمليات واستكشاف الأخطاء"),
        ["nav.regional_interop"] = ("Regional Interop", "المعايير الإقليمية"),
        ["nav.training_academy"] = ("Training Academy", "أكاديمية التدريب"),

        // Common Buttons & Actions
        ["btn.copy"] = ("Copy", "نسخ"),
        ["btn.copied"] = ("Copied!", "تم النسخ!"),
        ["btn.download"] = ("Download", "تحميل"),
        ["btn.reset"] = ("Reset", "إعادة ضبط"),
        ["btn.execute"] = ("Execute", "تنفيذ"),
        ["btn.inspect"] = ("Inspect", "فحص ومعاينة"),
        ["btn.validate"] = ("Validate", "التحقق من المطابقة"),
        ["btn.next_step"] = ("Next Step", "الخطوة التالية"),
        ["btn.prev_step"] = ("Previous Step", "الخطوة السابقة"),
        ["btn.restart"] = ("Restart Simulator", "إعادة تشغيل المحاكي"),
        ["btn.close"] = ("Close", "إغلاق"),
        ["btn.search"] = ("Search...", "بحث..."),
        ["btn.all"] = ("All", "الكل"),
        ["btn.clear"] = ("Clear", "مسح"),
        ["btn.print_certificate"] = ("Print / Export Certificate", "طباعة / تصدير الشهادة")
    };

    public void SetLanguage(string lang)
    {
        var normalized = lang.Equals("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";
        if (CurrentLanguage != normalized)
        {
            CurrentLanguage = normalized;
            OnLanguageChanged?.Invoke();
        }
    }

    public void ToggleLanguage()
    {
        SetLanguage(CurrentLanguage == "en" ? "ar" : "en");
    }

    public string T(string key, string? fallback = null)
    {
        if (_translations.TryGetValue(key, out var tuple))
        {
            return CurrentLanguage == "ar" ? tuple.ar : tuple.en;
        }

        return fallback ?? key;
    }
}
