using System;
using System.Reflection;
using System.Threading.Tasks;

using R5T.N0000;
using R5T.T0132;
using R5T.T0159;
using R5T.T0172;
using R5T.T0215;
using R5T.T0218;


namespace R5T.L0064.F001
{
    [FunctionalityMarker]
    public partial interface IDotnetPackPathOperator : IFunctionalityMarker,
        F0138.IDotnetPackPathOperator
    {
        public PairedAssemblyXmlDocumentionFilePaths Get_PairedAssemblyXmlDocumentationFilePaths(
            IDotnetPackName dotnetPackName,
            ITargetFrameworkMoniker targetFrameworkMoniker)
        {
            // Get the dotnet pack directory path.
            var dotnetDirectoryPath = this.Get_DotnetPackDirectoryPath(
                dotnetPackName,
                targetFrameworkMoniker);

            // Get the pairs of assembly file-documentation file outputs in the dotnet pack directory.
            var output = Instances.AssemblyFilePathOperator.Get_PairedAssemblyXmlDocumentationFilePaths(
                dotnetDirectoryPath);

            return output;
        }

        public void Foreach_DotnetPackAssemblyFile(
            IDotnetPackName dotnetPackName,
            ITargetFrameworkMoniker targetFrameworkMoniker,
            Action<IAssemblyFilePath, IDocumentationXmlFilePath> action)
        {
            var pairedFilePaths = this.Get_PairedAssemblyXmlDocumentationFilePaths(
                dotnetPackName,
                targetFrameworkMoniker);

            foreach (var pair in pairedFilePaths.PairedFilePaths)
            {
                action(pair.Key, pair.Value);
            }
        }

        public async Task Foreach_DotnetPackAssemblyFile(
            IDotnetPackName dotnetPackName,
            ITargetFrameworkMoniker targetFrameworkMoniker,
            Func<IAssemblyFilePath, IDocumentationXmlFilePath, Task> action)
        {
            var pairedFilePaths = this.Get_PairedAssemblyXmlDocumentationFilePaths(
                dotnetPackName,
                targetFrameworkMoniker);

            foreach (var pair in pairedFilePaths.PairedFilePaths)
            {
                await action(pair.Key, pair.Value);
            }
        }

        public void Foreach_MemberInfo(
            IDotnetPackName dotnetPackName,
            ITargetFrameworkMoniker targetFrameworkMoniker,
            ITextOutput textOutput,
            Action<MemberInfo, Assembly, IAssemblyFilePath, IDocumentationXmlFilePath> action)
        {
            this.Foreach_DotnetPackAssemblyFile(
                dotnetPackName,
                targetFrameworkMoniker,
                (assemblyFilePath, documentationFilePath) =>
                {
                    textOutput.WriteInformation($"Processing assembly...\n\t{assemblyFilePath}");

                    Instances.ReflectionOperator.In_AssemblyContext(
                        assemblyFilePath.Value,
                        assembly =>
                        {
                            Instances.AssemblyOperator.Foreach_Member(
                                assembly,
                                memberInfo =>
                                {
                                    action(memberInfo, assembly, assemblyFilePath, documentationFilePath);
                                });
                        });
                });
        }

        public void Foreach_MemberInfo(
            IDotnetPackName dotnetPackName,
            ITargetFrameworkMoniker targetFrameworkMoniker,
            Action<MemberInfo, Assembly, IAssemblyFilePath, IDocumentationXmlFilePath> action)
        {
            var textOutput = Instances.TextOutputOperator.Get_New_Null();

            this.Foreach_MemberInfo(
                dotnetPackName,
                targetFrameworkMoniker,
                textOutput,
                action);
        }

        public async Task Foreach_MemberInfo(
            IDotnetPackName dotnetPackName,
            ITargetFrameworkMoniker targetFrameworkMoniker,
            Func<MemberInfo, Assembly, IAssemblyFilePath, IDocumentationXmlFilePath, Task> action)
        {
            await this.Foreach_DotnetPackAssemblyFile(
                dotnetPackName,
                targetFrameworkMoniker,
                async (assemblyFilePath, documentationFilePath) =>
                {
                    await Instances.ReflectionOperator.In_AssemblyContext(
                        assemblyFilePath.Value,
                        async assembly =>
                        {
                            await Instances.AssemblyOperator.Foreach_Member(
                                assembly,
                                async memberInfo =>
                                {
                                    await action(memberInfo, assembly, assemblyFilePath, documentationFilePath);
                                });
                        });
                });
        }
    }
}
