Bu dosya, Visual Studio'nun projeyi nasıl oluşturduğunu açıklar.

Bu projeyi oluşturmak için aşağıdaki araçlar kullanıldı:
- Angular CLI (ng)

Bu projeyi oluşturmak için aşağıdaki adımlar kullanıldı:
- ng ile Angular projesi oluşturun: `ng new kocerbank-frontend --defaults --skip-install --skip-git --no-standalone `.
- angular.json dosyasını bağlantı noktasıyla güncelleştirin.
- Proje dosyasını (`kocerbank-frontend.esproj`) oluşturun.
- Hata ayıklamayı etkinleştirmek için `launch.json` dosyasını oluşturun.
- `jest-editor-support` eklemek için package.json dosyasını güncelleyin.
- Ana makineyi belirtmek için `package.json` dosyasındaki `start` betiğini güncelleyin.
- Birim testleri için `karma.conf.js` ekleyin.
- `angular.json` dosyasını `karma.conf.js` dosyasına yönlendirecek şekilde güncelle.
- Projeyi çözüme ekleyin.
- Bu dosyayı yazın.
