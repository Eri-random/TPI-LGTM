module.exports = function (config) {
    config.set({
      // base path that will be used to resolve all patterns (eg. files, exclude)
      basePath: '',
  
      // frameworks to use
      // available frameworks: https://www.npmjs.com/search?q=keywords:karma-adapter
      frameworks: ['jasmine', '@angular-devkit/build-angular'],
  
      // list of files / patterns to load in the browser
      files: [],
  
      // list of files / patterns to exclude
      exclude: [],
  
      // preprocess matching files before serving them to the browser
      // available preprocessors: https://www.npmjs.com/search?q=keywords:karma-preprocessor
      preprocessors: {},
  
      // test results reporter to use
      // possible values: 'dots', 'progress'
      // available reporters: https://www.npmjs.com/search?q=keywords:karma-reporter
      reporters: ['progress', 'kjhtml'],
  
      // web server port
      port: 9876,
  
      // enable / disable colors in the output (reporters and logs)
      colors: true,
  
      // level of logging
      // possible values: config.LOG_DISABLE || config.LOG_ERROR || config.LOG_WARN || config.LOG_INFO || config.LOG_DEBUG
      logLevel: config.LOG_INFO,
  
      // enable / disable watching file and executing tests whenever any file changes
      autoWatch: true,
  
      // start these browsers
      // available browser launchers: https://www.npmjs.com/search?q=keywords:karma-launcher
      browsers: ['ChromeHeadless'],
      customLaunchers: {
        ChromeHeadlessCustom: {
          base: 'ChromeHeadless',
          flags: ['--no-sandbox', '--disable-gpu']
        }
      },
  
      // Continuous Integration mode
      // if true, Karma captures browsers, runs the tests and exits
      singleRun: false,
  
      // Concurrency level
      // how many browser instances should be started simultaneously
      concurrency: Infinity,
  
      // Angular CLI plugin configuration
      client: {
        clearContext: false // leave Jasmine Spec Runner output visible in browser
      },
  
      // Configure the coverage reporter
      coverageIstanbulReporter: {
        dir: require('path').join(__dirname, './coverage'),
        reports: ['html', 'lcovonly', 'text-summary'],
        fixWebpackSourcePaths: true
      },
  
      // Karma plugins
      plugins: [
        require('karma-jasmine'),
        require('karma-chrome-launcher'),
        require('karma-jasmine-html-reporter'),
        require('karma-coverage-istanbul-reporter'),
        require('@angular-devkit/build-angular/plugins/karma')
      ]
    });
  };