import { TestBed } from '@angular/core/testing';

import { DonacionService } from './donacion.service';

describe('DonacionService', () => {
  let service: DonacionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DonacionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
