import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { UserStoreService } from '../services/user-store.service';

export const authGuard: CanActivateFn = (route, state) : boolean => {
  
  const authService = inject(AuthService);
  const userStore = inject(UserStoreService)
  const router = inject(Router);
  var isAuthenticated;
  var userRole;

  const expectedRole = route.data['expectedRole'];

  authService.getIsLoggedIn().subscribe(isLoggedIn => {
    const authLogin = authService.isLoggedIn();
    isAuthenticated = isLoggedIn || authLogin;
  });

  userStore.getRolFromStore()
  .subscribe(val =>{
    const roleFromToken = authService.getRoleFromToken();
    userRole = val || roleFromToken;
  });

  if (!isAuthenticated) {
    router.navigate(['/login']);
    return false;
  }

  if (expectedRole && expectedRole !== userRole) {
    router.navigate(['/not-found']);
    return false;
  }

  return true;
};
