var fs = require('fs'),
    path = require('path'),
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
task('default', ['build', 'nuget:pack'])

directory('Dist/')

namespace('build', function () {

    desc('Build Windows Store binaries')
    task('winstore', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-WindowsStore.sln',
            targets: ['Build']
        })
    }, { async: true })

    desc('Build Windows Phone 7.1 binaries')
    task('wp71', ['assemblyinfo:facebook'], function () {
        msbuild({
            file: 'Source/Facebook.Client-WP7.sln',
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

    task('all', ['build:wp71', 'build:wp8', 'build:winstore'])

    //task('mono', function (xbuildPath) {
    //    msbuild({
    //        _exe: xbuildPath || 'xbuild',
    //        file: 'Source/Facebook-Net40.sln',
    //        targets: ['Build'],
    //        properties: { TargetFrameworkProfile: '' }
    //    })
    //}, { async: true })

})

task('build', ['build:all'])

namespace('clean', function () {

    task('winstore', function () {
        msbuild({
            file: 'Source/Facebook.Client-WindowsStore.sln',
            targets: ['Clean']
        })
    }, { async: true })

    task('wp71', function () {
        msbuild({
            file: 'Source/Facebook.Client-WP7.sln',
            targets: ['Clean']
        })
    }, { async: true })

    task('wp8', function () {
        msbuild({
            file: 'Source/Facebook.Client-WP8.sln',
            targets: ['Clean']
        })
    }, { async: true })

    task('all', ['clean:wp71', 'clean:wp8', 'clean:winstore'])

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
