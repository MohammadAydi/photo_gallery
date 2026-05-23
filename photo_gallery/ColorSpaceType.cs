// هذا الملف كان في مشروعك الآخر باسم TESTcolorSystem.Enums
// التعديل الوحيد: تغيير الـ namespace إلى photo_gallery
// + إضافة HLS لأن الـ UI يحتوي على HLSButton

namespace photo_gallery;

public enum ColorSpaceType
{
    RGB,
    HSV,
    HLS,   // [إضافة] موجود في الـ UI (HLSButton) ولم يكن في الـ enum الأصلي
    LAB,
    YUV,
    YCbCr,
    CMYK
}