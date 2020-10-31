using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace NCoreUtils.Sass.Internal
{
    internal static class Interop
    {
        private const string Libname = "sass";

        internal static readonly UTF8Encoding _utf8 = new UTF8Encoding(false);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_copy_c_string(IntPtr @string);

        #region SassOptions

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_make_options();

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_delete_options(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_option_get_precision(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_option_get_output_style(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern bool sass_option_get_source_comments(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern bool sass_option_get_source_map_embed(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern bool sass_option_get_source_map_contents(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern bool sass_option_get_source_map_file_urls(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern bool sass_option_get_omit_source_map_url(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern bool sass_option_get_is_indented_syntax_src(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_indent(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_linefeed(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_input_path(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_output_path(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_source_map_file(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_source_map_root(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_c_functions(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_importer(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_option_get_include_path_size(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_include_path(IntPtr pOptions, int index);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_option_get_plugin_path_size(IntPtr pOptions);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_option_get_plugin_path(IntPtr pOptions, int index);


        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_precision(IntPtr pOptions, int precision);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_output_style(IntPtr pOptions, int output_style);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_source_comments(IntPtr pOptions, bool source_comments);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_source_map_embed(IntPtr pOptions, bool source_map_embed);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_source_map_contents(IntPtr pOptions, bool source_map_contents);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_source_map_file_urls(IntPtr pOptions, bool source_map_file_urls);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_omit_source_map_url(IntPtr pOptions, bool omit_source_map_url);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_is_indented_syntax_src(IntPtr pOptions, bool is_indented_syntax_src);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_indent(IntPtr pOptions, IntPtr indent);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_linefeed(IntPtr pOptions, IntPtr linefeed);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_input_path(IntPtr pOptions, IntPtr input_path);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_output_path(IntPtr pOptions, IntPtr output_path);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_plugin_path(IntPtr pOptions, IntPtr plugin_path);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_include_path(IntPtr pOptions, IntPtr include_path);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_source_map_file(IntPtr pOptions, IntPtr source_map_file);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_source_map_root(IntPtr pOptions, IntPtr source_map_root);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_c_functions(IntPtr pOptions, IntPtr c_functions);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_set_importer(IntPtr pOptions, IntPtr importer);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_push_plugin_path(IntPtr pOptions, IntPtr path);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_option_push_include_path(IntPtr pOptions, IntPtr path);

        #endregion

        #region Context

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_make_file_context(IntPtr input_path);

        /// <summary>
        /// NOTE: <c>source_string</c> is mutable!
        /// </summary>
        /// <param name="source_string">Sass source.</param>
        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_make_data_context(IntPtr source_string);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_delete_file_context(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_delete_data_context(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_compile_file_context(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_compile_data_context(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_file_context_get_options(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_data_context_get_options(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_file_context_set_options(IntPtr ctx, IntPtr options);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_data_context_set_options(IntPtr ctx, IntPtr options);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_file_context_get_context(IntPtr file_ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_data_context_get_context(IntPtr file_ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_output_string(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_context_get_error_status(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_error_json(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_error_text(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_error_message(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_error_file(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_error_src(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_context_get_error_line(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_context_get_error_column(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_source_map_string(IntPtr ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_context_get_included_files(IntPtr ctx);

        #endregion

        #region Compiler

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_make_file_compiler(IntPtr file_ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_make_data_compiler(IntPtr data_ctx);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_compiler_parse(IntPtr compiler);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_compiler_execute(IntPtr compiler);

        [DllImport(Libname, SetLastError = false)]
        public static extern void sass_delete_compiler(IntPtr compiler);

        [DllImport(Libname, SetLastError = false)]
        public static extern int sass_compiler_get_import_stack_size(IntPtr compiler);

        [DllImport(Libname, SetLastError = false)]
        public static extern IntPtr sass_compiler_get_import_entry(IntPtr compiler, int idx);

        #endregion

        #region string handling

        public unsafe static string? ReadOptionalUtf8String(IntPtr ptr)
        {
            if (IntPtr.Zero == ptr)
            {
                return default;
            }
            var p = (byte*)ptr;
            var i = 0;
            while (*(p + i) != 0)
            {
                ++i;
            }
            if (0 == i)
            {
                return string.Empty;
            }
            return _utf8.GetString(p, i);
        }

        public unsafe static string ReadUtf8String(IntPtr ptr)
        {
            var p = (byte*)ptr;
            var i = 0;
            while (*(p + i) != 0)
            {
                ++i;
            }
            if (0 == i)
            {
                return string.Empty;
            }
            return _utf8.GetString(p, i);
        }

        public unsafe static List<string> ReadUtf8StringList(IntPtr ptr)
        {
            var list = new List<string>();
            if (IntPtr.Zero == ptr)
            {
                return list;
            }
            for (var p = (byte**)ptr; *p != (byte*)0; ++p)
            {
                var line = ReadOptionalUtf8String((IntPtr)(*p));
                if (!(line is null))
                {
                    list.Add(line);
                }
            }
            return list;
        }

        public unsafe static void PassUtf8String(string input, Action<IntPtr> action)
        {
            if (input.Length == 0)
            {
                Span<byte> emptyBuffer = stackalloc byte[1];
                emptyBuffer[0] = 0;
                fixed (byte* ptr = emptyBuffer)
                {
                    action((IntPtr)ptr);
                }
            }
            else
            {
                var bufferSize = _utf8.GetMaxByteCount(input.Length);
                Span<byte> buffer = stackalloc byte[bufferSize + 1];
                var dataSize = _utf8.GetBytes(input.AsSpan(), buffer);
                buffer[dataSize] = 0;
                fixed (byte* ptr = buffer)
                {
                    action((IntPtr)ptr);
                }
            }
        }

        public unsafe static T PassUtf8String<T>(string input, Func<IntPtr, T> action)
        {
            if (input.Length == 0)
            {
                Span<byte> emptyBuffer = stackalloc byte[1];
                emptyBuffer[0] = 0;
                fixed (byte* ptr = emptyBuffer)
                {
                    return action((IntPtr)ptr);
                }
            }
            var bufferSize = _utf8.GetMaxByteCount(input.Length);
            Span<byte> buffer = stackalloc byte[bufferSize + 1];
            var dataSize = _utf8.GetBytes(input.AsSpan(), buffer);
            buffer[dataSize] = 0;
            fixed (byte* ptr = buffer)
            {
                return action((IntPtr)ptr);
            }
        }

        #endregion
    }
}