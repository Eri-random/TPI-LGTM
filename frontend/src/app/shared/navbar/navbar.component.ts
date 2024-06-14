import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { HeadquartersService } from 'src/app/services/headquarters.service';
import { UserStoreService } from 'src/app/services/user-store.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit{

  fullName!:string;
  role!:string;
  isLoggedIn:boolean=false;

  constructor(private userStore:UserStoreService,
    private authService:AuthService,
    private headquarterService: HeadquartersService,
  ){
  }

  ngOnInit(): void {
    this.authService.getIsLoggedIn().subscribe(isLoggedIn => {
      const authLogin = this.authService.isLoggedIn();
      this.isLoggedIn = isLoggedIn || authLogin;
    });
    
    this.userStore.getFullNameFromStore()
    .subscribe(val =>{
      const fullNameFromToken = this.authService.getFullNameFromToken();
      this.fullName = val || fullNameFromToken;
    })
  
    this.userStore.getRolFromStore()
    .subscribe(val =>{
      const roleFromToken = this.authService.getRoleFromToken();
      this.role = val || roleFromToken;
    })
  }

  logout(){
    this.authService.singOut();
    this.authService.setIsLoggedIn(false);
  }

  clearDataDirection() {
    this.headquarterService.clearDataDirection();
  }
}
