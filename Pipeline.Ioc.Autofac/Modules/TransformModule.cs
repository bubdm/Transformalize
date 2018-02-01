#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Cfg.Net.Contracts;
using Cfg.Net.Shorthand;
using JavaScriptEngineSwitcher.ChakraCore;
using Newtonsoft.Json;
using Transformalize.Contracts;
using Transformalize.Nulls;
using Transformalize.Providers.File.Transforms;
using Transformalize.Providers.Razor;
using Transformalize.Providers.Web;
using Transformalize.Transforms;
using Transformalize.Transforms.Compression;
using Transformalize.Transforms.CSharp;
using Transformalize.Transforms.DateMath;
using Transformalize.Transforms.Geography;
using Transformalize.Transforms.Html;
using Transformalize.Transforms.JavaScript;
using Transformalize.Transforms.Jint;
using Transformalize.Transforms.Json;
using Transformalize.Transforms.LamdaParser;
using Transformalize.Transforms.Vehicle;
using Transformalize.Transforms.Velocity;
using Transformalize.Transforms.Xml;
using Transformalize.Transforms.Globalization;
using Parameter = Cfg.Net.Shorthand.Parameter;
using Transformalize.Configuration;
using Transformalize.Context;
using Module = Autofac.Module;

namespace Transformalize.Ioc.Autofac.Modules {
    public class TransformModule : Module {

        public const string Name = "shorthand-t";
        private readonly HashSet<string> _methods = new HashSet<string>();
        private readonly ShorthandRoot _shortHand = new ShorthandRoot();
        private readonly Process _process;
        private readonly IPipelineLogger _logger;

        public TransformModule(Process process, IPipelineLogger logger) {
            _process = process;
            _logger = logger;
        }

        protected override void Load(ContainerBuilder builder) {

            var loadContext = new PipelineContext(_logger, _process);
            builder.Properties["ShortHand"] = _shortHand;
            builder.Properties["Methods"] = _methods;

            // new method so transform author can define shorthand signature(s)
            RegisterTransform(builder, c => new AbsTransform(c), new AbsTransform().GetSignatures());
            RegisterTransform(builder, c => new SliceTransform(c), new SliceTransform().GetSignatures());
            RegisterTransform(builder, c => new FormatTransform(c), new FormatTransform().GetSignatures());
            RegisterTransform(builder, c => new AddTransform(c), new AddTransform().GetSignatures());
            RegisterTransform(builder, c => new EqualsTransform(c), new EqualsTransform().GetSignatures());
            RegisterTransform(builder, c => new CeilingTransform(c), new CeilingTransform().GetSignatures());
            RegisterTransform(builder, c => new CoalesceTransform(c), new CoalesceTransform().GetSignatures());
            RegisterTransform(builder, c => new ConcatTransform(c), new ConcatTransform().GetSignatures());
            RegisterTransform(builder, c => new ConnectionTransform(c), new ConnectionTransform().GetSignatures());
            RegisterTransform(builder, c => new ConvertTransform(c), new ConvertTransform().GetSignatures());
            RegisterTransform(builder, c => new CopyTransform(c), new CopyTransform().GetSignatures());
            RegisterTransform(builder, c => new DateDiffTransform(c), new DateDiffTransform().GetSignatures());
            RegisterTransform(builder, c => new DatePartTransform(c), new DatePartTransform().GetSignatures());
            RegisterTransform(builder, c => new DecompressTransform(c), new DecompressTransform().GetSignatures());
            RegisterTransform(builder, c => new CompressTransform(c), new CompressTransform().GetSignatures());
            RegisterTransform(builder, c => new FileExtTransform(c), new FileExtTransform().GetSignatures());
            RegisterTransform(builder, c => new FileNameTransform(c), new FileNameTransform().GetSignatures());
            RegisterTransform(builder, c => new FilePathTransform(c), new FilePathTransform().GetSignatures());
            RegisterTransform(builder, c => new SplitTransform(c), new SplitTransform().GetSignatures());
            RegisterTransform(builder, c => new LastDayTransform(c), new LastDayTransform().GetSignatures());
            RegisterTransform(builder, c => new LastTransform(c), new LastTransform().GetSignatures());
            RegisterTransform(builder, c => new FirstTransform(c), new FirstTransform().GetSignatures());
            RegisterTransform(builder, c => new FloorTransform(c), new FloorTransform().GetSignatures());
            RegisterTransform(builder, c => new FormatXmlTransform(c), new FormatXmlTransform().GetSignatures());
            RegisterTransform(builder, c => new FormatPhoneTransform(c), new FormatPhoneTransform().GetSignatures());
            RegisterTransform(builder, c => new HashcodeTransform(c), new HashcodeTransform().GetSignatures());
            RegisterTransform(builder, c => new DecodeTransform(c), new DecodeTransform().GetSignatures());
            RegisterTransform(builder, c => new HtmlEncodeTransform(c), new HtmlEncodeTransform().GetSignatures());
            RegisterTransform(builder, c => new InsertTransform(c), new InsertTransform().GetSignatures());
            RegisterTransform(builder, c => new InvertTransform(c), new InvertTransform().GetSignatures());
            RegisterTransform(builder, c => new JoinTransform(c), new JoinTransform().GetSignatures());
            RegisterTransform(builder, c => new LeftTransform(c), new LeftTransform().GetSignatures());
            RegisterTransform(builder, c => new RightTransform(c), new RightTransform().GetSignatures());
            RegisterTransform(builder, c => new ToLowerTransform(c), new ToLowerTransform().GetSignatures());
            RegisterTransform(builder, c => new MapTransform(c), new MapTransform().GetSignatures());
            RegisterTransform(builder, c => new RegexMatchTransform(c), new RegexMatchTransform().GetSignatures());
            RegisterTransform(builder, c => new MultiplyTransform(c), new MultiplyTransform().GetSignatures());
            RegisterTransform(builder, c => new NextTransform(c), new NextTransform().GetSignatures());
            RegisterTransform(builder, c => new UtcNowTransform(c), new UtcNowTransform().GetSignatures());
            RegisterTransform(builder, c => new RowNumberTransform(c), new RowNumberTransform().GetSignatures());

            var pluginsFolder = Path.Combine(AssemblyDirectory, "plugins");
            if (Directory.Exists(pluginsFolder)) {

                var assemblies = new List<Assembly>();
                foreach (var file in Directory.GetFiles(pluginsFolder, "Transformalize.Transform.*.Autofac.dll", SearchOption.TopDirectoryOnly)) {
                    var info = new FileInfo(file);
                    var name = info.Name.ToLower().Split('.').FirstOrDefault(f => f != "dll" && f != "transformalize" && f != "transform" && f != "autofac");
                    loadContext.Debug(()=>$"Loading {name} transform");
                    var assembly = Assembly.LoadFile(new FileInfo(file).FullName);
                    assemblies.Add(assembly);
                }
                if (assemblies.Any()) {
                    builder.RegisterAssemblyModules(assemblies.ToArray());
                }
            }

            // register the short hand
            builder.Register((c, p) => _shortHand).Named<ShorthandRoot>(Name).InstancePerLifetimeScope();

            // old method
            builder.Register((c, p) => new PadLeftTransform(p.Positional<IContext>(0))).Named<ITransform>("padleft");
            builder.Register((c, p) => new PadRightTransform(p.Positional<IContext>(0))).Named<ITransform>("padright");
            builder.Register((c, p) => new RazorTransform(p.Positional<IContext>(0))).Named<ITransform>("razor");
            builder.Register((c, p) => new TimeZoneTransform(p.Positional<IContext>(0))).Named<ITransform>("timezone");

            builder.Register((c, p) => new RegexReplaceTransform(p.Positional<IContext>(0))).Named<ITransform>("regexreplace");
            builder.Register((c, p) => new RemoveTransform(p.Positional<IContext>(0))).Named<ITransform>("remove");
            builder.Register((c, p) => new ReplaceTransform(p.Positional<IContext>(0))).Named<ITransform>("replace");
            builder.Register((c, p) => new RoundTransform(p.Positional<IContext>(0))).Named<ITransform>("round");
            builder.Register((c, p) => new RoundToTransform(p.Positional<IContext>(0), RoundTo.Nearest)).Named<ITransform>("roundto");
            builder.Register((c, p) => new RoundToTransform(p.Positional<IContext>(0), RoundTo.Up)).Named<ITransform>("roundupto");
            builder.Register((c, p) => new RoundToTransform(p.Positional<IContext>(0), RoundTo.Down)).Named<ITransform>("rounddownto");
            builder.Register((c, p) => new SplitLengthTransform(p.Positional<IContext>(0))).Named<ITransform>("splitlength");
            builder.Register((c, p) => new SubStringTransform(p.Positional<IContext>(0))).Named<ITransform>("substring");
            builder.Register((c, p) => new TagTransform(p.Positional<IContext>(0))).Named<ITransform>("tag");
            builder.Register((c, p) => new RelativeTimeTransform(p.Positional<IContext>(0), true)).Named<ITransform>("timeago");
            builder.Register((c, p) => new RelativeTimeTransform(p.Positional<IContext>(0), false)).Named<ITransform>("timeahead");
            builder.Register((c, p) => new ToStringTransform(p.Positional<IContext>(0))).Named<ITransform>("tostring");
            builder.Register((c, p) => new ToTimeTransform(p.Positional<IContext>(0))).Named<ITransform>("totime");
            builder.Register((c, p) => new ToYesNoTransform(p.Positional<IContext>(0))).Named<ITransform>("toyesno");
            builder.Register((c, p) => new TrimTransform(p.Positional<IContext>(0))).Named<ITransform>("trim");
            builder.Register((c, p) => new TrimEndTransform(p.Positional<IContext>(0))).Named<ITransform>("trimend");
            builder.Register((c, p) => new TrimStartTransform(p.Positional<IContext>(0))).Named<ITransform>("trimstart");
            builder.Register((c, p) => new VelocityTransform(p.Positional<IContext>(0), c.Resolve<IReader>())).Named<ITransform>("velocity");
            builder.Register((c, p) => new ToUpperTransform(p.Positional<IContext>(0))).Named<ITransform>("upper");
            builder.Register((c, p) => new ToUpperTransform(p.Positional<IContext>(0))).Named<ITransform>("toupper");

            builder.Register((c, p) => new XPathTransform(p.Positional<IContext>(0))).Named<ITransform>("xpath");
            builder.Register((c, p) => new IIfTransform(p.Positional<IContext>(0))).Named<ITransform>("iif");
            builder.Register((c, p) => new GeohashEncodeTransform(p.Positional<IContext>(0))).Named<ITransform>("geohashencode");
            builder.Register((c, p) => new GeohashNeighborTransform(p.Positional<IContext>(0))).Named<ITransform>("geohashneighbor");
            builder.Register((c, p) => new CommonPrefixTransform(p.Positional<IContext>(0))).Named<ITransform>("commonprefix");
            builder.Register((c, p) => new CommonPrefixesTransform(p.Positional<IContext>(0))).Named<ITransform>("commonprefixes");
            builder.Register((c, p) => new DistanceTransform(p.Positional<IContext>(0))).Named<ITransform>("distance");

            builder.Register((c, p) => new FilterTransform(p.Positional<IContext>(0), FilterType.Include)).Named<ITransform>("include");
            builder.Register((c, p) => new FilterTransform(p.Positional<IContext>(0), FilterType.Exclude)).Named<ITransform>("exclude");

            builder.Register((c, p) => new DateAddTransform(p.Positional<IContext>(0))).Named<ITransform>("dateadd");
            builder.Register((c, p) => new FromSplitTransform(p.Positional<IContext>(0))).Named<ITransform>("fromsplit");
            builder.Register((c, p) => new FromRegexTransform(p.Positional<IContext>(0))).Named<ITransform>("fromregex");
            builder.Register((c, p) => new FromLengthsTranform(p.Positional<IContext>(0))).Named<ITransform>("fromlengths");

            // return true or false, validators
            builder.Register((c, p) => new AnyTransform(p.Positional<IContext>(0))).Named<ITransform>("any");
            builder.Register((c, p) => new StartsWithTransform(p.Positional<IContext>(0))).Named<ITransform>("startswith");
            builder.Register((c, p) => new EndsWithTransform(p.Positional<IContext>(0))).Named<ITransform>("endswith");
            builder.Register((c, p) => new InTransform(p.Positional<IContext>(0))).Named<ITransform>("in");
            builder.Register((c, p) => new ContainsTransform(p.Positional<IContext>(0))).Named<ITransform>("contains");
            builder.Register((c, p) => new IsTransform(p.Positional<IContext>(0))).Named<ITransform>("is");
            builder.Register((c, p) => new EqualsTransform(p.Positional<IContext>(0))).Named<ITransform>("equal");
            builder.Register((c, p) => new EqualsTransform(p.Positional<IContext>(0))).Named<ITransform>("equals");
            builder.Register((c, p) => new IsEmptyTransform(p.Positional<IContext>(0))).Named<ITransform>("isempty");
            builder.Register((c, p) => new IsDefaultTransform(p.Positional<IContext>(0))).Named<ITransform>("isdefault");
            builder.Register((c, p) => new IsNumericTransform(p.Positional<IContext>(0))).Named<ITransform>("isnumeric");
            builder.Register((c, p) => new RegexIsMatchTransform(p.Positional<IContext>(0))).Named<ITransform>("ismatch");

            builder.Register((c, p) => new GeocodeTransform(p.Positional<IContext>(0))).Named<ITransform>("fromaddress");
            builder.Register((c, p) => new DateMathTransform(p.Positional<IContext>(0))).Named<ITransform>("datemath");
            builder.Register((c, p) => new IsDaylightSavingsTransform(p.Positional<IContext>(0))).Named<ITransform>("isdaylightsavings");
            builder.Register((c, p) => new SlugifyTransform(p.Positional<IContext>(0))).Named<ITransform>("slugify");

            /* VIN, Vehicle Identification Number, note: you get red intellisense here because vin library is portable */
            builder.Register((c, p) => new VinValidateTransform(p.Positional<IContext>(0))).Named<ITransform>("isvin");
            builder.Register((c, p) => new VinValidateTransform(p.Positional<IContext>(0))).Named<ITransform>("vinisvalid");
            builder.Register((c, p) => new VinGetWorldManufacturerTransform(p.Positional<IContext>(0))).Named<ITransform>("vingetworldmanufacturer");
            builder.Register((c, p) => new VinGetModelYearTransform(p.Positional<IContext>(0))).Named<ITransform>("vingetmodelyear");

            // wip
            builder.Register((c, p) => new WebTransform(p.Positional<IContext>(0))).Named<ITransform>("web");
            builder.Register((c, p) => new UrlEncodeTransform(p.Positional<IContext>(0))).Named<ITransform>("urlencode");
            builder.Register((c, p) => new FromJsonTransform(p.Positional<IContext>(0), o => JsonConvert.SerializeObject(o, Formatting.None))).Named<ITransform>("fromjson");

            builder.Register((c, p) => new LamdaParserEvalTransform(p.Positional<IContext>(0))).Named<ITransform>("eval");
            builder.Register((c, p) => new DistinctTransform(p.Positional<IContext>(0))).Named<ITransform>("distinct");
            builder.Register((c, p) => new RegexMatchCountTransform(p.Positional<IContext>(0))).Named<ITransform>("matchcount");



            builder.Register((c, p) => new AppendTransform(p.Positional<IContext>(0))).Named<ITransform>("append");
            builder.Register((c, p) => new PrependTransform(p.Positional<IContext>(0))).Named<ITransform>("prepend");

            builder.Register((c, p) => {
                var context = p.Positional<IContext>(0);
                return context.Operation.XmlMode == "all" || context.Field.Engine != "auto" ?
                    new Transforms.Xml.FromXmlTransform(context, c.ResolveNamed<IRowFactory>(context.Entity.Key, new NamedParameter("capacity", context.GetAllEntityFields().Count()))) :
                    new Transforms.FromXmlTransform(context) as ITransform;
            }).Named<ITransform>("fromxml");

            builder.Register<ITransform>((c, p) => {
                var context = p.Positional<IContext>(0);
                if (c.ResolveNamed<IHost>("cs").Start()) {
                    return new CsharpTransform(context);
                }
                context.Error("Unable to register csharp transform");
                return new NullTransform(context);
            }).Named<ITransform>("cs");
            builder.Register((c, p) => c.ResolveNamed<ITransform>("cs", p)).Named<ITransform>("csharp");


            builder.Register((c, p) => {
                var context = p.Positional<IContext>(0);
                var strict = new HashSet<string>(new[] { "int", "int32", "string", "bool", "boolean", "double" });
                switch (context.Field.Engine) {
                    case "auto":
                        var fields = context.Entity.GetFieldMatches(context.Operation.Script);
                        return fields.All(f => strict.Contains(f.Type)) ?
                            (ITransform)new JavascriptTransform(new ChakraCoreJsEngineFactory(), context, c.Resolve<IReader>()) :
                            new JintTransform(c.Resolve<IReader>(), context);
                    case "jint":
                        return new JintTransform(c.Resolve<IReader>(), context);
                    default:
                        return new JavascriptTransform(new ChakraCoreJsEngineFactory(), context, c.Resolve<IReader>());
                }
            }).Named<ITransform>("js");
            builder.Register((c, p) => c.ResolveNamed<ITransform>("js", p)).Named<ITransform>("javascript");

        }

        private void RegisterTransform(ContainerBuilder builder, Func<IContext, ITransform> getTransform, IEnumerable<OperationSignature> signatures) {

            foreach (var s in signatures) {
                if (_methods.Add(s.Method)) {

                    var method = new Method { Name = s.Method, Signature = s.Method, Ignore = s.Ignore };
                    _shortHand.Methods.Add(method);

                    var signature = new Signature {
                        Name = s.Method,
                        NamedParameterIndicator = s.NamedParameterIndicator
                    };

                    foreach (var parameter in s.Parameters) {
                        signature.Parameters.Add(new Parameter {
                            Name = parameter.Name,
                            Value = parameter.Value
                        });
                    }
                    _shortHand.Signatures.Add(signature);
                }

                builder.Register((c, p) => getTransform(p.Positional<IContext>(0))).Named<ITransform>(s.Method);
            }

        }

        public static string AssemblyDirectory {
            get {
                var codeBase = typeof(Process).Assembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
