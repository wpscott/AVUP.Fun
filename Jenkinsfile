pipeline {
	agent any
	stages {
		stage('PreBuild') {
			steps {
				sh 'docker pull mcr.microsoft.com/dotnet/aspnet:5.0'
				sh 'docker pull mcr.microsoft.com/dotnet/sdk:5.0'
			}
		}

		stage('Build') {
			steps {
				script {
					sh "docker build -t \"127.0.0.1:42266/avupfun\" --label \"com.microsoft.created-by=visual-studio\" --label \"com.microsoft.visual-studio.project-name=AVUP.Fun\" -f ./AVUP.Fun/Dockerfile ."
					sh "docker build -t \"127.0.0.1:42266/avupfunintake\" --label \"com.microsoft.created-by=visual-studio\" --label \"com.microsoft.visual-studio.project-name=AVUP.Fun.Intake\" -f ./AVUP.Fun.Intake/Dockerfile ."
					sh "docker build -t \"127.0.0.1:42266/avupfunprocess\" --label \"com.microsoft.created-by=visual-studio\" --label \"com.microsoft.visual-studio.project-name=AVUP.Fun.Process\" -f ./AVUP.Fun.Process/Dockerfile ."
				}
			}
		}

		stage('Publish') {
			steps {
				script {
					sh "docker push 127.0.0.1:42266/avupfun"
					sh "docker push 127.0.0.1:42266/avupfunintake"
					sh "docker push 127.0.0.1:42266/avupfunprocess"
				}
			}
		}
	}
}