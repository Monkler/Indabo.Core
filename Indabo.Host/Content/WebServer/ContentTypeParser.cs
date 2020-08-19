namespace Indabo.Host
{
    internal static class ContentTypeParser
    {
        private static string[,,] contentTypeMappings = new string[,,] { {
            { "text/calendar", "ics", "iCalendar-Dateien" },
            { "text/comma-separated-values", "csv", "kommaseparierte Datendateien" },
            { "text/css", "css", "CSS Stylesheet-Dateien" },
            { "text/html", "htm", "HTML-Dateien" },
            { "text/html", "html", "HTML-Dateien" },
            { "text/html", "shtml", "HTML-Dateien" },
            { "text/javascript", "js", "JavaScript-Dateien" },
            { "text/plain", "txt", "reine Textdateien" },
            { "text/richtext", "rtx", "Richtext-Dateien" },
            { "text/rtf", "rtf", "RTF-Dateien" },
            { "text/tab-separated-values", "tsv", "tabulator-separierte Datendateien" },
            { "text/vnd.wap.wml", "wml", "WML-Dateien (WAP)" },
            { "application/vnd.wap.wmlc", "wmlc", "WMLC-Dateien (WAP)" },
            { "text/vnd.wap.wmlscript", "wmls", "WML-Scriptdateien (WAP)" },
            { "application/vnd.wap.wmlscriptc", "wmlsc", "WML-Script-C-dateien (WAP)" },
            { "text/xml", "xml", "XML-Dateien" },
            { "text/x-setext", "etx", "SeText-Dateien" },
            { "text/x-sgml", "sgm", "SGML-Dateien" },
            { "text/x-speech", "talk", "Speech-Dateien" },

            { "image/bmp", "bmp", "Windows Bitmap-Datei" },
            { "image/cis-cod", "cod", "CIS-Cod-Dateien" },
            { "image/cmu-raster", "ras", "CMU-Raster-Dateien" },
            { "image/fif", "fif", "FIF-Dateien" },
            { "image/gif", "gif", "GIF-Dateien" },
            { "image/ief", "ief", "IEF-Dateien" },
            { "image/jpeg", "jpeg", "JPEG-Dateien" },
            { "image/jpeg", "jpg", "JPEG-Dateien" },
            { "image/png", "png", "PNG-Dateien" },
            { "image/svg+xml", "svg", "SVG-Dateien" },
            { "image/tiff", "tiff", "TIFF-Dateien" },
            { "image/vasa", "mcf", "Vasa-Dateien" },
            { "image/vnd.wap.wbmp", "wbmp", "Bitmap-Dateien (WAP)" },
            { "image/x-freehand", "fh4", "Freehand-Dateien" },
            { "image/x-icon", "ico", "Icon-Dateien (z.B. Favoriten-Icons)" },
            { "image/x-portable-anymap", "pnm", "PBM Anymap Dateien" },
            { "image/x-portable-bitmap", "pbm", "PBM Bitmap Dateien" },
            { "image/x-portable-graymap", "pgm", "PBM Graymap Dateien" },
            { "image/x-portable-pixmap", "ppm", "PBM Pixmap Dateien" },
            { "image/x-rgb", "rgb", "RGB-Dateien" },
            { "image/x-windowdump", "xwd", "X-Windows Dump" },
            { "image/x-xbitmap", "xbm", "XBM-Dateien" },
            { "image/x-xpixmap", "xpm", "XPM-Dateien" },

            { "application/acad", "dwg", "AutoCAD-Dateien (nach NCSA)" },
            { "application/astound", "asd", "Astound-Dateien" },
            { "application/dsptype", "tsp", "TSP-Dateien" },
            { "application/dxf", "dxf", "AutoCAD-Dateien (nach CERN)" },
            { "application/force-download", "reg", "Registrierungsdateien" },
            { "application/futuresplash", "spl", "Flash Futuresplash-Dateien" },
            { "application/gzip", "gz", "GNU Zip-Dateien" },
            { "application/javascript", "js", "serverseitige JavaScript-Dateien" },
            { "application/json", "json", "enthält einen String in JavaScript-Objekt-Notation" },
            { "application/listenup", "ptlk", "Listenup-Dateien" },
            { "application/mac-binhex40", "hqx", "Macintosh Binärdateien" },
            { "application/mbedlet", "mbd", "Mbedlet-Dateien" },
            { "application/mif", "mif", "FrameMaker Interchange Format Dateien" },
            { "application/msexcel", "xls", "Microsoft Excel Dateien" },
            { "application/mshelp", "hlp", "Microsoft Windows Hilfe Dateien" },
            { "application/mshelp", "chm", "Microsoft Windows Hilfe Dateien" },
            { "application/mspowerpoint", "ppt", "Microsoft Powerpoint Dateien" },
            { "application/msword", "doc", "Microsoft Word Dateien" },
            { "application/octet-stream", "bin", "Nicht näher spezifizierte Daten, z.B. ausführbare Dateien" },
            { "application/oda", "oda", "Oda-Dateien" },
            { "application/pdf", "pdf", "PDF-Dateien" },
            { "application/postscript", "ai", "PostScript-Dateien" },
            { "application/rtc", "rtc", "RTC-Dateien" },
            { "application/rtf", "rtf", "RTF-Dateien" },
            { "application/studiom", "smp", "Studiom-Dateien" },
            { "application/toolbook", "tbk", "Toolbook-Dateien" },
            { "application/vocaltec-media-desc", "vmd", "Vocaltec Mediadesc-Dateien" },
            { "application/vocaltec-media-file", "vmf", "Vocaltec Media-Dateien" },
            { "application/vnd.openxmlformats-officedocument. spreadsheetml.sheet", "xlsx", "Excel (OpenOffice Calc)" },
            { "application/vnd.openxmlformats-officedocument. wordprocessingml.document", "docx", "Word (OpenOffice Writer)" },
            { "application/xml", "xml", "XML-Dateien" },
            { "application/x-bcpio", "bcpio", "BCPIO-Dateien" },
            { "application/x-compress", "z", "zlib-komprimierte Dateien" },
            { "application/x-cpio", "cpio", "CPIO-Dateien" },
            { "application/x-csh", "csh", "C-Shellscript-Dateien" },
            { "application/x-director", "dcr", "Macromedia Director-Dateien" },
            { "application/x-dvi", "dvi", "DVI-Dateien" },
            { "application/x-envoy", "evy", "Envoy-Dateien" },
            { "application/x-gtar", "gtar", "GNU tar-Archivdateien" },
            { "application/x-hdf", "hdf", "HDF-Dateien" },
            { "application/x-httpd-php", "php phtml", "PHP-Dateien" },
            { "application/x-latex", "latex", "LaTeX-Quelldateien" },
            { "application/x-macbinary", "bin", "Macintosh Binärdateien" },
            { "application/x-mif", "mif", "FrameMaker Interchange Format Dateien" },
            { "application/x-netcdf", "nc", "Unidata CDF-Dateien" },
            { "application/x-nschat", "nsc", "NS Chat-Dateien" },
            { "application/x-sh", "sh", "Bourne Shellscript-Dateien" },
            { "application/x-shar", "shar", "Shell-Archivdateien" },
            { "application/x-shockwave-flash", "swf", "Flash Shockwave-Dateien" },
            { "application/x-sprite", "spr", "Sprite-Dateien" },
            { "application/x-stuffit", "sit", "Stuffit-Dateien" },
            { "application/x-supercard", "sca", "Supercard-Dateien" },
            { "application/x-sv4cpio", "sv4cpio", "CPIO-Dateien" },
            { "application/x-sv4crc", "sv4crc", "CPIO-Dateien mit CRC" },
            { "application/x-tar", "tar", "tar-Archivdateien" },
            { "application/x-tcl", "tcl", "TCL Scriptdateien" },
            { "application/x-tex", "tex", "TeX-Dateien" },
            { "application/x-texinfo", "texinfo", "Texinfo-Dateien" },
            { "application/x-troff", "t tr", "TROFF-Dateien (Unix)" },
            { "application/x-troff-man", "man", "TROFF-Dateien mit MAN-Makros (Unix)" },
            { "application/x-troff-me", "me", "TROFF-Dateien mit ME-Makros (Unix)" },
            { "application/x-ustar", "ustar", "tar-Archivdateien (Posix)" },
            { "application/x-wais-source", "src", "WAIS Quelldateien" },
            { "application/zip", "zip", "ZIP-Archivdateien" },
            { "audio/basic", "au snd", "Sound-Dateien" },
            { "audio/echospeech", "es", "Echospeed-Dateien" },
            { "audio/mpeg", "mp3", "MP3-Dateien" },
            { "audio/mp4", "mp4", "MP4-Dateien" },
            { "audio/ogg", "ogg", "OGG-Dateien" },
            { "audio/tsplayer", "tsi", "TS-Player-Dateien" },
            { "audio/voxware", "vox", "Vox-Dateien" },
            { "audio/wav", "wav", "WAV-Dateien" },
            { "audio/x-aiff", "aif", "AIFF-Sound-Dateien" },
            { "audio/x-dspeeh", "dus", "Sprachdateien" },
            { "audio/x-midi", "mid", "MIDI-Dateien" },
            { "audio/x-mpeg", "mp2", "MPEG-Audiodateien" },
            { "audio/x-pn-realaudio", "ram", "RealAudio-Dateien" },
            { "audio/x-pn-realaudio-plugin", "rpm", "RealAudio-Plugin-Dateien" },
            { "audio/x-qt-stream", "stream", "Quicktime-Streaming-Dateien" }
        } };

        public static string GetContentTypeFromFileName(string fileName)
        {
            //foreach (string contentType in contentTypeMappings.)
            for (int i = 0; i < contentTypeMappings.GetLength(1); i++)
            {
                if (fileName.EndsWith(contentTypeMappings[0, i, 1]))
                {
                    return contentTypeMappings[0, i, 0];
                }
            }

            return null;
        }

        public static string GetContentTypeFromFileEnding(string ending)
        {
            for (int i = 0; i < contentTypeMappings.GetLength(1); i++)
            {
                if (contentTypeMappings[0, i, 1] == ending)
                {
                    return contentTypeMappings[0, i, 0];
                }
            }

            return null;
        }
    }
}
