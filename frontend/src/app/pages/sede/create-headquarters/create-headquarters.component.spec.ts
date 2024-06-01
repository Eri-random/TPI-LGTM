import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateHeadquartersComponent } from './create-headquarters.component';

describe('CrearSedeComponent', () => {
  let component: CreateHeadquartersComponent;
  let fixture: ComponentFixture<CreateHeadquartersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CreateHeadquartersComponent]
    });
    fixture = TestBed.createComponent(CreateHeadquartersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
