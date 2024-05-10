import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
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
    private authService:AuthService
  ){
  }

  ngOnInit(): void {
    // this.authService.getIsLoggedIn().subscribe(isLoggedIn => {
    //   this.isLoggedIn = isLoggedIn;
    // });
    
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
}
