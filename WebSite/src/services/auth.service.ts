import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { LoginService } from '../services/index';

@Injectable({
  providedIn: 'root'
})
export class AuthService implements CanActivate {

  constructor(private loginService: LoginService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {

    if (this.loginService.getToken() == null || this.loginService.getToken() == '') {
      this.router.navigate(['login']);
      return false;
    } else if (!this.loginService.isAdmin() && (state.url.includes('lookups') || state.url.includes('resources'))) {
      this.router.navigate(['login']);
      return false;
    } else {  
    }

    return true;
  }

}
