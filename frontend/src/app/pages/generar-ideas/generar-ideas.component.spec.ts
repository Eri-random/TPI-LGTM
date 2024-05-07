import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GenerarIdeasComponent } from './generar-ideas.component';

describe('GenerarIdeasComponent', () => {
  let component: GenerarIdeasComponent;
  let fixture: ComponentFixture<GenerarIdeasComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [GenerarIdeasComponent]
    });
    fixture = TestBed.createComponent(GenerarIdeasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
