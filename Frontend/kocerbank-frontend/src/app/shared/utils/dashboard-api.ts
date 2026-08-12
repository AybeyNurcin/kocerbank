import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { DashboardFiltre } from '../models/dashboard-filtre-model';

/*
 * Müşteri/Şube/Personel/Hesap API servislerinin dördünde de
 * birebir aynı olan "DashboardOzet" çağrısı. Diğer CRUD
 * metotları (listele/ekle/guncelle) entity'ye göre farklı
 * URL/payload şekilleri kullandığından burada tekilleştirilmedi.
 */
export function dashboardOzetGetir<T>(
  http: HttpClient,
  apiUrl: string,
  filtre?: DashboardFiltre
): Observable<T> {

  return http.post<T>(
    `${apiUrl}/DashboardOzet`,
    filtre ?? null
  );
}
