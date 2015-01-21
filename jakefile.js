// You WILL need to set the VisualStudioVersion variable to 12.0 on bash command prompt
// $ export VisualStudioVersion=12.0
var fs = require('fs'),
    path = require('path'),
    exec = require('child_process').exec,
    njake = require('./Build/njake'),
    msbuild = njake.msbuild,
    xunit = njake.xunit,
    nuget = njake.nuget,
    assemblyinfo = njake.assemblyinfo,
    config = {
        rootPath: __dirname,
        version: fs.readFileSync('VERSION', 'utf-8').split('\r\n')[0],
        fileVersion: fs.readFileSync('VERSION', 'utf-8').split('\r\n')[1]
    };
    docConfig = {
        docConfigFilePath: "doc-config.json",
        outputDir: "doc_output"
    };

console.log('Facebook C# SDK for Windows & Windows Phone v' + config.version + ' (' + config.fileVersion + ')')

msbuild.setDefaults({
    properties: { Configuration: 'Release' },
    processor: 'x86',
    version: 'net4.0'
})

xunit.setDefaults({
    _exe: 'Source/packages/xunit.runners.1.9.0.1566/tools/xunit.console.clr4.x86'
})

nuget.setDefaults({
    _exe: 'Source/.nuget/NuGet.exe',
    verbose: true
})

assemblyinfo.setDefaults({
    language: 'c#'
})

desc('Build all binaries, run tests and create nuget and symbolsource packages')
//task('default', ['build', 'test', 'nuget:pack'])
task('default', ['build', 'nuget:pack', 'sdkreference:generate'])

directory('Dist/')

namespace('build', function () {

    desc('Build Windows Store binaries')
    task('winstore', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-WindowsStore.sln',
            targets: ['Build']
        })
    }, { async: true })
	
	desc('Build Windows Universal binaries')
    task('windows_universal', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-Universal/Facebook.Client-Universal.Windows/Facebook.Client-Universal.Windows.sln',
            targets: ['Build']
        })
    }, { async: true })

    desc('Build Windows Phone Universal binaries')
    task('phone_universal', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-Universal/Facebook.Client-Universal.WindowsPhone/Facebook.Client-Universal.WindowsPhone.sln',
            targets: ['Build']
        })
    }, { async: true })
    
    desc('Build Windows Phone 8 binaries')
    task('wp8', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-WP8.sln',
            targets: ['Build']
        })
    }, { async: true })

    task('all', ['build:wp8', 'build:winstore', 'build:windows_universal', 'build:phone_universal'])


})

task('build', ['build:all'])

namespace('clean', function () {

    task('winstore', function () {
        msbuild({
            file: 'Source/Facebook.Client-WindowsStore.sln',
            targets: ['Clean']
        })
    }, { async: true })

    task('windows_universal', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-Universal/Facebook.Client-Universal.Windows/Facebook.Client-Universal.Windows.sln',
            targets: ['Clean']
        })
    }, { async: true })


    task('phone_universal', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-Universal/Facebook.Client-Universal.WindowsPhone/Facebook.Client-Universal.WindowsPhone.sln',
            targets: ['Clean']
        })
    }, { async: true })

    task('wp8', function () {
        msbuild({
            file: 'Source/Facebook.Client-WP8.sln',
            targets: ['Clean']
        })
    }, { async: true })

    task('all', ['clean:wp8', 'clean:winstore', 'clean:windows_universal', 'clean:phone_universal'])

})

desc('Clean all')
task('clean', ['clean:all'], function () {
    jake.rmRf('Working/')
    jake.rmRf('Dist/')
})

namespace('tests', function () {

    task('winstore', ['build:winstore'], function () {
        //xunit({
        //    assembly: 'Bin/Tests/Release/Facebook.Client.Tests.dll'
        //})
    }, { async: true })

    task('all', ['tests:winstore'])

})

desc('Run tests')
task('test', ['tests:all'])

directory('Dist/NuGet', ['Dist/'])
directory('Dist/SymbolSource', ['Dist/'])

namespace('nuget', function () {

    namespace('pack', function () {

        task('nuget', ['Dist/NuGet', 'build'], function () {
            nuget.pack({
                nuspec: 'Build/NuGet/Facebook.Client/Facebook.Client.nuspec',
                version: config.fileVersion,
                outputDirectory: 'Dist/NuGet'
            })
        }, { async: true })


        task('symbolsource', ['Dist/SymbolSource', 'build'], function () {
            nuget.pack({
                nuspec: 'Build/SymbolSource/Facebook.Client/Facebook.Client.nuspec',
                version: config.fileVersion,
                outputDirectory: 'Dist/SymbolSource'
            })
        }, { async: true })

        task('all', ['nuget:pack:nuget', 'nuget:pack:symbolsource'])

    })

    namespace('push', function () {

        desc('Push nuget package to nuget.org')
        task('nuget', function(apiKey) {
            nuget.push({
                apiKey: apiKey,
                package: path.join(config.rootPath, 'Dist/NuGet/Facebook.Client.' + config.fileVersion + '.nupkg')
            })
        }, { async: true })

        desc('Push nuget package to symbolsource')
        task('symbolsource', function(apiKey) {
            nuget.push({
                apiKey: apiKey,
                package: path.join(config.rootPath, 'Dist/SymbolSource/Facebook.Client.' + config.fileVersion + '.nupkg'),
                source: nuget.sources.symbolSource
            })
        }, { async: true })

    })

    desc('Create NuGet and SymbolSource pacakges')
    task('pack', ['nuget:pack:all'])

})

namespace('assemblyinfo', function () {

    task('facebook', function () {
        assemblyinfo({
            file: 'Source/Facebook.Client/Properties/AssemblyInfo.cs',
            assembly: {
                notice: function () {
                    return '// Do not modify this file manually, use jakefile instead.\r\n';
                },
                AssemblyTitle: 'Facebook',
                AssemblyDescription: 'Facebook SDK for Windows & Windows Phone',
                AssemblyCompany: 'The Outercurve Foundation',
                AssemblyProduct: 'Facebook SDK for Windows & Windows Phone',
                AssemblyCopyright: 'Copyright (c) 2011, The Outercurve Foundation.',
                ComVisible: false,
                AssemblyVersion: config.version,
                AssemblyFileVersion: config.fileVersion
            }
        })
    }, { async: true })

})

namespace('sdkreference', function () {

    desc('Generate the SDK Reference documentation')
    task('generate',['sdkreference:clean'], function () {
        console.log('generate doc');
        console.log('Generating using the "%s" config file.', path.resolve(docConfig.docConfigFilePath));
        console.log('Output directory: "%s".', path.resolve(docConfig.outputDir) + '\\');

        if (!fs.existsSync(docConfig.outputDir)){
            fs.mkdirSync(docConfig.outputDir);
            
        }

        var command = 'netdoc fromConfig ' + path.resolve(docConfig.docConfigFilePath) + ' ' + path.resolve(docConfig.outputDir) + '\\' ;
        exec(command, function dir(error, stdout, stderr) { console.log(stdout); });
        
    }, { async: true })

    desc('Remove the SDK Reference documentation')
    task('clean', function () {
        console.log('clean doc');
        if (fs.existsSync(docConfig.outputDir)){
            var files = fs.readdirSync(docConfig.outputDir);
            for(var i = 0; i < files.length; i++) {
                fs.unlinkSync(path.join(docConfig.outputDir,files[i]));
            }

            fs.rmdirSync(docConfig.outputDir);
        }
    })

})
