import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditHeadquartersComponent } from './edit-headquarters.component';

describe('EditarSedeComponent', () => {
  let component: EditHeadquartersComponent;
  let fixture: ComponentFixture<EditHeadquartersComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [EditHeadquartersComponent]
    });
    fixture = TestBed.createComponent(EditHeadquartersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
