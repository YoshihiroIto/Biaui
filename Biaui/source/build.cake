#tool nuget:?package=xunit.runner.console&version=2.4.1
#tool Squirrel.Windows&version=1.9.0
#addin Cake.Squirrel&version=0.14.0

var target = Argument("target", "Build");
var configuration = Argument("configuration", "Debug");

string currentConfiguration = null;


///////////////////////////////////////////////////////////////////////////////////////////////////
Task("Build")
    .Does(() =>
    {
        NuGetRestore("./Biaui.sln");

        MSBuild("./Biaui.sln", settings => settings.SetConfiguration(currentConfiguration));
    });


///////////////////////////////////////////////////////////////////////////////////////////////////
Task("UnitTest")
    .IsDependentOn("Build")
    .Does(() =>
    {
        XUnit2(GetFiles($"./Biaui.*/**/bin/{currentConfiguration}/**/*.Test.dll"));
    });


///////////////////////////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
    {
        var targets = new []{"./Biaui.*/**/bin", "./Biaui.*/**/obj"};
        var s = new DeleteDirectorySettings { Recursive = true, Force = true };

        foreach(var t in targets)
        {
            CleanDirectories(t);
            //DeleteDirectories(GetDirectories(t), s);
        }
    });


///////////////////////////////////////////////////////////////////////////////////////////////////
Task("Pack")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var outputDir = @"..\artifacts\";

        var versionRegex = new System.Text.RegularExpressions.Regex(@"(\d+).(\d+).(\d+).(\d+)");
        var v = versionRegex.Match(ParseAssemblyInfo("./Biaui.Gui.Windows/Properties/AssemblyInfo.cs").AssemblyVersion);
        var version = $"{v.Groups[1]}.{v.Groups[2]}.{v.Groups[3]}";
        var outputNupkg = $"{outputDir}Biaui.{version}.nupkg";

        // 
        // nugetパッケージを作る
        // 
        {
            var s = new NuGetPackSettings
            {
                OutputDirectory = outputDir,
                IncludeReferencedProjects = true,
                Version = version,
                Properties = new Dictionary<string, string>
                {
                    { "Configuration", currentConfiguration }
                }
            };

            var baseDir = MakeAbsolute(Directory($"./Biaui.Gui.Windows/bin/{currentConfiguration}")).ToString();

            s.Files = GetFiles(baseDir + "/**/*.*")
                .Select(f => f.FullPath)
                .Where(f => System.IO.Path.GetExtension(f).ToLower() != ".pdb")
                .Select(f =>
                {
                    var subDir = f.Substring(baseDir.Length);

                    return new NuSpecContent {Source = f, Target = @"lib\net471" + subDir};
                })
                .ToArray();


            NuGetPack("./Biaui.Gui.Windows/Biaui.Gui.Windows.nuspec", s);
        }


        // 
        // Squirrelにかける
        //
        {
            var s = new SquirrelSettings
            {
                ReleaseDirectory = outputDir + "Releases",
            };
            Squirrel(File(outputNupkg), s);
        }
    });



foreach(var c in configuration.Split(';'))
{
    currentConfiguration = c;
    RunTarget(target);
}

